using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Data.Dtos;
using Project1.Services;

namespace Project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;

        public AuthController(IAuthService authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] UserRegister dto, IFormFile? image)
        {
            string? imagePath = null;

            if (image != null)
            {
                var directory = Path.Combine("Data", "Static", "Images");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(directory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                imagePath = Path.Combine(fileName);
            }

            var result = await _authService.Register(dto, imagePath);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin dto)
        {
            var (isSuccess, message, token) = await _authService.Login(dto);

            if (!isSuccess)
            {
                return Unauthorized(new { message });
            }

            return Ok(new { message, token });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userIdString = userIdClaim.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == userIdString
            );

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(
                new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Image,
                    user.CreatedAt,
                    user.UpdatedAt,
                }
            );
        }

        [HttpPut("edit-profile")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdate dto, IFormFile? image)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userIdString = userIdClaim.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == userIdString
            );

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!string.IsNullOrWhiteSpace(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (image != null)
            {
                var directory = Path.Combine("Data", "Static", "Images");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(directory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                user.Image = Path.Combine(fileName);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    message = "Profile updated successfully",
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Image,
                        user.CreatedAt,
                        user.UpdatedAt,
                    },
                }
            );
        }
    }
}
