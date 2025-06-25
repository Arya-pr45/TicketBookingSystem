using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Application.DTOs;

namespace TicketBookingWebApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly TicketBookingSystemContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(TicketBookingSystemContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = dto.Role
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            return GenerateToken(user);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var jwtKey = _config["JwtSettings:SecretKey"];
            var jwtIssuer = _config["JwtSettings:Issuer"];
            var jwtAudience = _config["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured.");
            if (string.IsNullOrEmpty(jwtIssuer))
                throw new InvalidOperationException("JWT Issuer is not configured.");
            if (string.IsNullOrEmpty(jwtAudience))
                throw new InvalidOperationException("JWT Audience is not configured.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = user.Role,
                UserName = user.UserName,
                Success = true,
                Message = "Authentication successful"
            };
        }
    }
}