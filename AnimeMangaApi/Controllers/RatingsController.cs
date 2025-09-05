using System.Security.Claims;
using AnimeMangaApi.Data;
using AnimeMangaApi.DTOs;
using AnimeMangaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimeMangaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RatingsController(AppDbContext db) => _db = db;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(RatingCreateDto dto)
        {
            // Get current user id from JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            // Ensure target entry exists
            var entryExists = await _db.AnimeMangaEntries.AnyAsync(e => e.Id == dto.AnimeMangaEntryId);
            if (!entryExists) return NotFound(new { message = "Anime/Manga entry not found." });

            var rating = new Rating
            {
                AnimeMangaEntryId = dto.AnimeMangaEntryId,
                UserId = userId,
                Score = dto.Score,
                Comment = dto.Comment
            };

            _db.Ratings.Add(rating);
            await _db.SaveChangesAsync();
            return Ok(rating);
        }
    }
}
