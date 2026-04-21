using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Moq;
using backend.Controllers;
using backend.Repositories;
using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace backend.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserRepository> _mockUsers;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUsers = new Mock<IUserRepository>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["JwtKey"]).Returns("supersecretkey1234567890");

            _controller = new AuthController(_mockUsers.Object, _mockConfig.Object);
        }

        // ---------------------------------------------------------
        // REGISTER
        // ---------------------------------------------------------

        [Fact]
        public async Task Register_UsernameExists_ReturnsBadRequest()
        {
            _mockUsers.Setup(r => r.UsernameExistsAsync("test")).ReturnsAsync(true);

            var dto = new UserRegisterDto
            {
                Username = "test",
                Email = "email@test.com",
                Password = "123"
            };

            var result = await _controller.Register(dto) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Username already exists", result.Value);
        }

        [Fact]
        public async Task Register_EmailExists_ReturnsBadRequest()
        {
            _mockUsers.Setup(r => r.UsernameExistsAsync("newuser")).ReturnsAsync(false);
            _mockUsers.Setup(r => r.EmailExistsAsync("email@test.com")).ReturnsAsync(true);

            var dto = new UserRegisterDto
            {
                Username = "newuser",
                Email = "email@test.com",
                Password = "123"
            };

            var result = await _controller.Register(dto) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Email already exists", result.Value);
        }

        [Fact]
        public async Task Register_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.UsernameExistsAsync("x"))
                      .ThrowsAsync(new Exception("DB error"));

            var dto = new UserRegisterDto
            {
                Username = "x",
                Email = "x@test.com",
                Password = "123"
            };

            var result = await _controller.Register(dto);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // VERIFY EMAIL
        // ---------------------------------------------------------

        [Fact]
        public async Task VerifyEmail_ValidToken_ReturnsOk()
        {
            var user = new User
            {
                Id = "1",
                EmailVerificationToken = "abc",
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(1)
            };

            _mockUsers.Setup(r => r.GetByEmailVerificationTokenAsync("abc"))
                      .ReturnsAsync(user);

            var result = await _controller.VerifyEmail("abc") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Email verified", result.Value);
            Assert.True(user.EmailConfirmed);
            _mockUsers.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task VerifyEmail_InvalidToken_ReturnsBadRequest()
        {
            _mockUsers.Setup(r => r.GetByEmailVerificationTokenAsync("wrong"))
                      .ReturnsAsync((User?)null);

            var result = await _controller.VerifyEmail("wrong") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid or expired token", result.Value);
        }

        [Fact]
        public async Task VerifyEmail_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.GetByEmailVerificationTokenAsync("abc"))
                      .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.VerifyEmail("abc");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // LOGIN
        // ---------------------------------------------------------

        [Fact]
        public async Task Login_UserNotFound_ReturnsUnauthorized()
        {
            _mockUsers.Setup(r => r.GetByUsernameAsync("missing"))
                      .ReturnsAsync((User?)null);

            var dto = new UserLoginDto { Username = "missing", Password = "123" };

            var result = await _controller.Login(dto) as UnauthorizedObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid username or password", result.Value);
        }

        [Fact]
        public async Task Login_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.GetByUsernameAsync("x"))
                      .ThrowsAsync(new Exception("DB error"));

            var dto = new UserLoginDto { Username = "x", Password = "123" };

            var result = await _controller.Login(dto);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // REQUEST PASSWORD RESET
        // ---------------------------------------------------------

        [Fact]
        public async Task RequestPasswordReset_EmailNotFound_ReturnsOkGeneric()
        {
            _mockUsers.Setup(r => r.GetByEmailAsync("missing@test.com"))
                      .ReturnsAsync((User?)null);

            var dto = new PasswordResetRequestDto { Email = "missing@test.com" };

            var result = await _controller.RequestPasswordReset(dto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("If the email exists, a reset token will be returned.", result.Value);
        }

        [Fact]
        public async Task RequestPasswordReset_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.GetByEmailAsync("x@test.com"))
                      .ThrowsAsync(new Exception("DB error"));

            var dto = new PasswordResetRequestDto { Email = "x@test.com" };

            var result = await _controller.RequestPasswordReset(dto);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // RESET PASSWORD
        // ---------------------------------------------------------

        [Fact]
        public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
        {
            _mockUsers.Setup(r => r.GetByPasswordResetTokenAsync("bad"))
                      .ReturnsAsync((User?)null);

            var dto = new PasswordResetDto { Token = "bad", NewPassword = "newpass" };

            var result = await _controller.ResetPassword(dto) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid or expired token", result.Value);
        }

        [Fact]
        public async Task ResetPassword_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.GetByPasswordResetTokenAsync("x"))
                      .ThrowsAsync(new Exception("DB error"));

            var dto = new PasswordResetDto { Token = "x", NewPassword = "newpass" };

            var result = await _controller.ResetPassword(dto);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // CHANGE PASSWORD
        // ---------------------------------------------------------

        [Fact]
        public async Task ChangePassword_WrongCurrentPassword_ReturnsUnauthorized()
        {
            var user = new User
            {
                Id = "1",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 1, 2, 3 }
            };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "wrong",
                NewPassword = "newpass"
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", "1")
                    }))
                }
            };

            var result = await _controller.ChangePassword(dto) as UnauthorizedObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Current password incorrect", result.Value);
        }

        [Fact]
        public async Task ChangePassword_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1"))
                      .ThrowsAsync(new Exception("DB error"));

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "x",
                NewPassword = "y"
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", "1")
                    }))
                }
            };

            var result = await _controller.ChangePassword(dto);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // DELETE ACCOUNT
        // ---------------------------------------------------------

        [Fact]
        public async Task DeleteAccount_ReturnsOk()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", "1")
                    }))
                }
            };

            var result = await _controller.DeleteAccount() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Account deleted", result.Value);
            _mockUsers.Verify(r => r.DeleteAsync("1"), Times.Once);
        }

        [Fact]
        public async Task DeleteAccount_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.DeleteAsync("1"))
                      .ThrowsAsync(new Exception("DB error"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", "1")
                    }))
                }
            };

            var result = await _controller.DeleteAccount();

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}
