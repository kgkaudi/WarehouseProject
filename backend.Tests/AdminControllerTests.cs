using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using backend.Controllers;
using backend.Repositories;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<IUserRepository> _mockUsers;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockUsers = new Mock<IUserRepository>();
            _controller = new AdminController(_mockUsers.Object);
        }

        // ---------------------------------------------------------
        // GET /api/admin/users
        // ---------------------------------------------------------

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            var users = new List<User>
            {
                new User { Id = "1", Username = "A" },
                new User { Id = "2", Username = "B" }
            };

            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _controller.GetAllUsers() as OkObjectResult;

            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<User>>(result.Value);
            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task GetAllUsers_RepositoryThrows_Returns500()
        {
            _mockUsers.Setup(r => r.GetAllAsync())
                      .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.GetAllUsers();

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // PUT /api/admin/promote/{id}
        // ---------------------------------------------------------

        [Fact]
        public async Task PromoteToAdmin_UserExists_ReturnsOk()
        {
            var user = new User { Id = "1", Role = "user" };
            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _controller.PromoteToAdmin("1") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User promoted to admin", result.Value);
            Assert.Equal("admin", user.Role);
            _mockUsers.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task PromoteToAdmin_UserNotFound_ReturnsNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

            var result = await _controller.PromoteToAdmin("missing") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User not found", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_InvalidId_ReturnsNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync(""))
                      .ReturnsAsync((User?)null);

            var result = await _controller.PromoteToAdmin("") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User not found", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_RepositoryThrowsOnGet_Returns500()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1"))
                      .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.PromoteToAdmin("1");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task PromoteToAdmin_RepositoryThrowsOnUpdate_Returns500()
        {
            var user = new User { Id = "1", Role = "user" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user))
                      .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.PromoteToAdmin("1");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // ---------------------------------------------------------
        // DELETE /api/admin/delete/{id}
        // ---------------------------------------------------------

        [Fact]
        public async Task DeleteUser_UserExists_ReturnsOk()
        {
            var user = new User { Id = "1" };
            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _controller.DeleteUser("1") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User deleted", result.Value);
            _mockUsers.Verify(r => r.DeleteAsync("1"), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

            var result = await _controller.DeleteUser("missing") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User not found", result.Value);
        }

        [Fact]
        public async Task DeleteUser_InvalidId_ReturnsNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync(""))
                      .ReturnsAsync((User?)null);

            var result = await _controller.DeleteUser("") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User not found", result.Value);
        }

        [Fact]
        public async Task DeleteUser_RepositoryThrowsOnGet_Returns500()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1"))
                      .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.DeleteUser("1");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_RepositoryThrowsOnDelete_Returns500()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.DeleteAsync("1"))
                      .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.DeleteUser("1");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}
