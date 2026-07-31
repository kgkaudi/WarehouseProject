using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.DTOs;
using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using backend.Repositories;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository users, IConfiguration config)
        {
            _users = users;
            _config = config;
        }

        // ---------------------------------------------------------
        // REGISTER
        // ---------------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid request");

                if (string.IsNullOrWhiteSpace(dto.Username) ||
                    string.IsNullOrWhiteSpace(dto.Email) ||
                    string.IsNullOrWhiteSpace(dto.Password) ||
                    string.IsNullOrWhiteSpace(dto.CompanyName) ||
                    string.IsNullOrWhiteSpace(dto.CompanyAddress))
                    return BadRequest("All fields are required");

                if (await _users.UsernameExistsAsync(dto.Username))
                    return BadRequest("Username already exists");

                if (await _users.EmailExistsAsync(dto.Email))
                    return BadRequest("Email already exists");

                CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

                var verificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(), // FIX: prevents _id: null
                    Username = dto.Username,
                    Email = dto.Email,
                    CompanyName = dto.CompanyName,
                    CompanyAddress = dto.CompanyAddress,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    EmailVerificationToken = verificationToken,
                    EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                    Role = "user",
                    EmailConfirmed = false
                };

                await _users.CreateAsync(user);

                return Ok(new
                {
                    message = "User registered. Verify email using the token.",
                    verificationToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // VERIFY EMAIL
        // ---------------------------------------------------------
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest("Token required");

                var user = await _users.GetByEmailVerificationTokenAsync(token);

                if (user == null ||
                    user.EmailVerificationTokenExpires == null ||
                    user.EmailVerificationTokenExpires <= DateTime.UtcNow)
                    return BadRequest("Invalid or expired token");

                if (user.EmailConfirmed)
                    return Ok("Email already verified");

                user.EmailConfirmed = true;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpires = null;

                await _users.UpdateAsync(user);

                return Ok("Email verified");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // LOGIN
        // ---------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid request");

                if (string.IsNullOrWhiteSpace(dto.Identifier) ||
                    string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest("Identifier and password required");

                // Try username first
                var user = await _users.GetByUsernameAsync(dto.Identifier);

                // If not found, try email
                if (user == null)
                    user = await _users.GetByEmailAsync(dto.Identifier);

                if (user == null)
                    return Unauthorized("Invalid username/email or password");

                if (user.PasswordHash == null || user.PasswordSalt == null)
                    return StatusCode(500, "Malformed user credentials");

                if (!VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                    return Unauthorized("Invalid username/email or password");

                if (!user.EmailConfirmed)
                    return Unauthorized("Email not verified");

                if (string.IsNullOrWhiteSpace(_config["JwtKey"]))
                    return StatusCode(500, "JWT key missing");

                var token = GenerateJwtToken(user);

                return Ok(new { token, username = user.Username, role = user.Role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // REQUEST PASSWORD RESET
        // ---------------------------------------------------------
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest("Email required");

                var user = await _users.GetByEmailAsync(dto.Email);

                if (user == null)
                    return Ok("If the email exists, a reset token will be returned.");

                user.PasswordResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
                user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

                await _users.UpdateAsync(user);

                return Ok(new
                {
                    message = "Password reset token generated.",
                    resetToken = user.PasswordResetToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // RESET PASSWORD
        // ---------------------------------------------------------
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(PasswordResetDto dto)
        {
            try
            {
                if (dto == null ||
                    string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrWhiteSpace(dto.NewPassword))
                    return BadRequest("Token and new password required");

                var user = await _users.GetByPasswordResetTokenAsync(dto.Token);

                if (user == null ||
                    user.PasswordResetTokenExpires == null ||
                    user.PasswordResetTokenExpires <= DateTime.UtcNow)
                    return BadRequest("Invalid or expired token");

                CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);

                user.PasswordHash = hash;
                user.PasswordSalt = salt;
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpires = null;

                await _users.UpdateAsync(user);

                return Ok("Password reset successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // CHANGE PASSWORD
        // ---------------------------------------------------------
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                if (dto == null ||
                    string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                    string.IsNullOrWhiteSpace(dto.NewPassword))
                    return BadRequest("Current and new password required");

                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                var userId = userIdClaim.Value;
                var user = await _users.GetByIdAsync(userId);

                if (user == null)
                    return Unauthorized("User not found");

                if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                    return Unauthorized("Current password incorrect");

                CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);

                user.PasswordHash = hash;
                user.PasswordSalt = salt;

                await _users.UpdateAsync(user);

                return Ok("Password changed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // DELETE ACCOUNT
        // ---------------------------------------------------------
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                var userId = userIdClaim.Value;

                var deleted = await _users.DeleteAsync(userId);

                if (!deleted)
                    return StatusCode(500, "Delete failed");

                return Ok("Account deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------
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
            var keyString = _config["JwtKey"];
            if (string.IsNullOrWhiteSpace(keyString))
                throw new Exception("JWT key missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Username", user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
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
