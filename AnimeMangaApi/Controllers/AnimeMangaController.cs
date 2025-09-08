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
    public class AnimeMangaController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AnimeMangaController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.AnimeMangaEntries
                                .Select(e => new
                                {
                                    e.Id,
                                    e.Title,
                                    e.Type,
                                    e.Year,
                                    RatingsCount = e.Ratings.Count,
                                    AvgScore = e.Ratings.Count == 0 ? 0 : e.Ratings.Average(r => r.Score)
                                })
                                .ToListAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(AnimeMangaCreateDto dto)
        {
            var entry = new AnimeMangaEntry
            {
                Title = dto.Title,
                Type = dto.Type,
                Year = dto.Year
            };
            _db.AnimeMangaEntries.Add(entry);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entry = await _db.AnimeMangaEntries
                                 .Include(e => e.Ratings)
                                 .FirstOrDefaultAsync(e => e.Id == id);
            if (entry == null) return NotFound();
            return Ok(entry);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var entry = await _db.AnimeMangaEntries
                                 .Include(e => e.Ratings)
                                 .FirstOrDefaultAsync(e => e.Id == id);
            if (entry == null) return NotFound();
            _db.AnimeMangaEntries.Remove(entry);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}