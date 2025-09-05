using AnimeMangaApi.Data;
using AnimeMangaApi.DTOs;
using AnimeMangaApi.Models;
using AnimeMangaApi.Services;
using BCrypt.Net;
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, user = new { user.Id, user.Username } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized(new { message = "Invalid credentials." });

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return Unauthorized(new { message = "Invalid credentials." });

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, user = new { user.Id, user.Username } });
        }
    }
}