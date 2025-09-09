using AnimeMangaApi.Data;
using AnimeMangaApi.DTOs;
using AnimeMangaApi.Models;
using AnimeMangaApi.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimeMangaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var exists = await _db.Users.AnyAsync(u => u.Username == dto.Username);
            if (exists) return Conflict(new { message = "Username already taken." });

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User" // default role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, user = new { user.Id, user.Username, user.Role } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized(new { message = "Invalid credentials." });

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return Unauthorized(new { message = "Invalid credentials." });

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, user = new { user.Id, user.Username, user.Role } });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] string newRole)
        {
            var allowedRoles = new[] { "User", "Admin" };
            if (!allowedRoles.Contains(newRole))
                return BadRequest(new { message = "Invalid role." });

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = newRole;
            await _db.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, user.Role });
        }
    }
}