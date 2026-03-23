using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.DTOs;
using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using backend.Service;
using MongoDB.Driver;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _mongo;
        private readonly IConfiguration _config;

        public AuthController(MongoDbService mongo, IConfiguration config)
        {
            _mongo = mongo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _mongo.Users.Find(u => u.Username == dto.Username).AnyAsync())
                return BadRequest("Username already exists");

            if (await _mongo.Users.Find(u => u.Email == dto.Email).AnyAsync())
                return BadRequest("Email already exists");

            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

            var verificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                CompanyName = dto.CompanyName,
                CompanyAddress = dto.CompanyAddress,
                PasswordHash = hash,
                PasswordSalt = salt,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24)
            };

            await _mongo.Users.InsertOneAsync(user);

            return Ok(new
            {
                message = "User registered. Verify email using the token.",
                verificationToken
            });
        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var user = await _mongo.Users.Find(u =>
                u.EmailVerificationToken == token &&
                u.EmailVerificationTokenExpires > DateTime.UtcNow).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Invalid or expired token");

            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpires = null;

            await _mongo.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok("Email verified");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _mongo.Users.Find(u => u.Username == dto.Username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized("Invalid username or password");

            if (!VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid username or password");

            if (!user.EmailConfirmed)
                return Unauthorized("Email not verified");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestDto dto)
        {
            var user = await _mongo.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null)
                return Ok("If the email exists, a reset token will be returned.");

            user.PasswordResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _mongo.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok(new
            {
                message = "Password reset token generated.",
                resetToken = user.PasswordResetToken
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(PasswordResetDto dto)
        {
            var user = await _mongo.Users.Find(u =>
                u.PasswordResetToken == dto.Token &&
                u.PasswordResetTokenExpires > DateTime.UtcNow).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Invalid or expired token");

            CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            await _mongo.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok("Password reset successful");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirst("UserId")!.Value;
            var user = await _mongo.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

            if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Current password incorrect");

            CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _mongo.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok("Password changed");
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst("UserId")!.Value;

            await _mongo.Users.DeleteOneAsync(u => u.Id == userId);

            return Ok("Account deleted");
        }


        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(hash);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtKey"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Username", user.Username)
        };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}