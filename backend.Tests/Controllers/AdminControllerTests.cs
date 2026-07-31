using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using backend.Controllers;
using backend.Service;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _mockService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockService = new Mock<IAdminService>();
            _controller = new AdminController(_mockService.Object);
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

            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.GetAllUsers() as OkObjectResult;

            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<User>>(result.Value);
            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task GetAllUsers_EmptyList_ReturnsOkWithEmptyList()
        {
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<User>());

            var result = await _controller.GetAllUsers() as OkObjectResult;

            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<User>>(result.Value);
            Assert.Empty(returned);
        }

        [Fact]
        public async Task GetAllUsers_ServiceReturnsNull_Returns500()
        {
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync((IEnumerable<User>)null);

            var result = await _controller.GetAllUsers();
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ServiceThrows_Returns500()
        {
            _mockService.Setup(s => s.GetAllUsersAsync())
                        .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.GetAllUsers();

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ListContainsNullEntry_Returns500()
        {
            var users = new List<User?>
            {
                new User { Id = "1", Username = "A" },
                null
            };

            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users!);

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
            _mockService.Setup(s => s.PromoteToAdminAsync("1")).ReturnsAsync(true);

            var result = await _controller.PromoteToAdmin("1") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User promoted to admin", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_UserNotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.PromoteToAdminAsync("missing")).ReturnsAsync(false);

            var result = await _controller.PromoteToAdmin("missing") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User not found", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_EmptyId_ReturnsBadRequest()
        {
            var result = await _controller.PromoteToAdmin("") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid user id", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_NullId_ReturnsBadRequest()
        {
            var result = await _controller.PromoteToAdmin(null!) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid user id", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_WhitespaceId_ReturnsBadRequest()
        {
            var result = await _controller.PromoteToAdmin("   ") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid user id", result.Value);
        }

        [Fact]
        public async Task PromoteToAdmin_ServiceThrows_Returns500()
        {
            _mockService.Setup(s => s.PromoteToAdminAsync("1"))
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
            _mockService.Setup(s => s.DeleteUserAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteUser("1") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User deleted", result.Value);
        }

        [Fact]
        public async Task DeleteUser_DeleteFails_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteUserAsync("missing")).ReturnsAsync(false);

            var result = await _controller.DeleteUser("missing") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("User not found or delete failed", result.Value);
        }

        [Fact]
        public async Task DeleteUser_EmptyId_ReturnsBadRequest()
        {
            var result = await _controller.DeleteUser("") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid user id", result.Value);
        }

        [Fact]
        public async Task DeleteUser_NullId_ReturnsBadRequest()
        {
            var result = await _controller.DeleteUser(null!) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid user id", result.Value);
        }

        [Fact]
        public async Task DeleteUser_WhitespaceId_ReturnsBadRequest()
        {
            var result = await _controller.DeleteUser("   ") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid user id", result.Value);
        }

        [Fact]
        public async Task DeleteUser_ServiceThrows_Returns500()
        {
            _mockService.Setup(s => s.DeleteUserAsync("1"))
                        .ThrowsAsync(new System.Exception("DB error"));

            var result = await _controller.DeleteUser("1");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}