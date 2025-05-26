using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Data.Dtos;
using Project1.Models;

namespace Project1.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> Register(UserRegister dto, string? imagePath)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return "Email already exists.";
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = hashedPassword,
                Image = imagePath,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<(bool IsSuccess, string Message, string? Token)> Login(UserLogin dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == dto.Username || u.Email == dto.Username
            );

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return (false, "Invalid username/email or password.", null);
            }

            var token = _jwtService.GenerateToken(user);
            return (true, "Login successful.", token);
        }
    }
}
