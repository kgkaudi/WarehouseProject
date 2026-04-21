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
                if (await _users.UsernameExistsAsync(dto.Username))
                    return BadRequest("Username already exists");

                if (await _users.EmailExistsAsync(dto.Email))
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
                var user = await _users.GetByEmailVerificationTokenAsync(token);

                if (user == null || user.EmailVerificationTokenExpires <= DateTime.UtcNow)
                    return BadRequest("Invalid or expired token");

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
                var user = await _users.GetByUsernameAsync(dto.Username);
                if (user == null)
                    return Unauthorized("Invalid username or password");

                if (!VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                    return Unauthorized("Invalid username or password");

                if (!user.EmailConfirmed)
                    return Unauthorized("Email not verified");

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
                var user = await _users.GetByPasswordResetTokenAsync(dto.Token);

                if (user == null || user.PasswordResetTokenExpires <= DateTime.UtcNow)
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
                var userId = User.FindFirst("UserId")!.Value;
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
                var userId = User.FindFirst("UserId")!.Value;

                await _users.DeleteAsync(userId);

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
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtKey"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Username", user.Username),
                new Claim(ClaimTypes.Role, user.Role)
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
