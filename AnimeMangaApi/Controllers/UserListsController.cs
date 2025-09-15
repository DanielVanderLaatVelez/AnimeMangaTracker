using AnimeMangaApi.Data;
using AnimeMangaApi.DTOs;
using AnimeMangaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AnimeMangaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // only logged-in users can manage lists
    public class UserListsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UserListsController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Anyone can view a user's list
        [AllowAnonymous]
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserListByUsername(string username)
        {
            var user = await _db.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null) return NotFound(new { message = "User not found." });

            var list = await _db.UserLists
                .Include(l => l.AnimeMangaEntry)
                .Where(l => l.UserId == user.Id)
                .Select(l => new UserListReadDto
                {
                    Id = l.Id,
                    Status = l.Status,
                    Progress = l.Progress,
                    Notes = l.Notes,
                    Title = l.AnimeMangaEntry.Title,
                    Type = l.AnimeMangaEntry.Type,
                    Year = l.AnimeMangaEntry.Year
                })
                .ToListAsync();

            return Ok(new { user = user.Username, list });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyList()
        {
            var userId = GetUserId();
            var list = await _db.UserLists
                .Include(l => l.AnimeMangaEntry)
                .Where(l => l.UserId == userId)
                .Select(l => new UserListReadDto
                {
                    Id = l.Id,
                    Status = l.Status,
                    Progress = l.Progress,
                    Notes = l.Notes,
                    Title = l.AnimeMangaEntry.Title,
                    Type = l.AnimeMangaEntry.Type,
                    Year = l.AnimeMangaEntry.Year
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type)
        {
            var userId = GetUserId();

            var list = await _db.UserLists
                .Include(l => l.AnimeMangaEntry)
                .Where(l => l.UserId == userId && l.AnimeMangaEntry.Type.ToLower() == type.ToLower())
                .Select(l => new UserListReadDto
                {
                    Id = l.Id,
                    Status = l.Status,
                    Progress = l.Progress,
                    Notes = l.Notes,
                    Title = l.AnimeMangaEntry.Title,
                    Type = l.AnimeMangaEntry.Type,
                    Year = l.AnimeMangaEntry.Year
                })
                .ToListAsync();

            if (!list.Any()) 
                return NotFound(new { message = $"No {type} entries found for this user." });

            return Ok(list);
        }


        [HttpPost]
        public async Task<IActionResult> AddToList(UserListDto dto)
        {
            var userId = GetUserId();

            var animeMangaEntry = await _db.AnimeMangaEntries
                .FirstOrDefaultAsync(e => e.Title.ToLower() == dto.Title.ToLower());

            if (animeMangaEntry == null)
                return NotFound(new { message = $"Anime/Manga with title '{dto.Title}' not found." });

            var exists = await _db.UserLists
                .AnyAsync(l => l.UserId == userId && l.AnimeMangaEntryId == animeMangaEntry.Id);

            if (exists) return BadRequest(new { message = "Already in your list." });

            var entry = new UserList
            {
                UserId = userId,
                AnimeMangaEntryId = animeMangaEntry.Id,
                Status = dto.Status,
                Progress = dto.Progress,
                Notes = dto.Notes
            };

            _db.UserLists.Add(entry);
            await _db.SaveChangesAsync();

            return Ok(new UserListReadDto
            {
                Id = entry.Id,
                Status = entry.Status,
                Progress = entry.Progress,
                Notes = entry.Notes,
                Title = animeMangaEntry.Title,
                Type = animeMangaEntry.Type,
                Year = animeMangaEntry.Year
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListEntry(int id, UserListDto dto)
        {
            var userId = GetUserId();
            var entry = await _db.UserLists
                .Include(l => l.AnimeMangaEntry)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (entry == null) return NotFound();

            entry.Status = dto.Status;
            entry.Progress = dto.Progress;
            entry.Notes = dto.Notes;

            await _db.SaveChangesAsync();

            return Ok(new UserListReadDto
            {
                Id = entry.Id,
                Status = entry.Status,
                Progress = entry.Progress,
                Notes = entry.Notes,
                Title = entry.AnimeMangaEntry.Title,
                Type = entry.AnimeMangaEntry.Type,
                Year = entry.AnimeMangaEntry.Year
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromList(int id)
        {
            var userId = GetUserId();
            var entry = await _db.UserLists
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (entry == null) return NotFound();

            _db.UserLists.Remove(entry);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}