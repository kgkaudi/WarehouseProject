using backend.Models;
using backend.Repositories;
using backend.Service;
using Moq;

namespace backend.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IUserRepository> _mockUsers;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            _mockUsers = new Mock<IUserRepository>();
            _service = new AdminService(_mockUsers.Object);
        }

        // ---------------------------------------------------------
        // GET ALL USERS
        // ---------------------------------------------------------
        [Fact]
        public async Task GetAllUsers_ReturnsUsers()
        {
            var users = new List<User>
            {
                new User { Id = "1", Username = "kostas" },
                new User { Id = "2", Username = "admin" }
            };

            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _service.GetAllUsersAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllUsers_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
        {
            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            var result = await _service.GetAllUsersAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsEmptyList_WhenRepositoryReturnsNull()
        {
            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync((List<User>?)null);

            var result = await _service.GetAllUsersAsync();

            Assert.Null(result); // service does not transform null → empty
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListWithNullEntries()
        {
            var users = new List<User?> { new User { Id = "1" }, null };

            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users!);

            var result = await _service.GetAllUsersAsync();

            Assert.NotNull(result);
            Assert.Contains(result, u => u == null);
        }

        [Fact]
        public async Task GetAllUsers_Throws_WhenRepositoryThrows()
        {
            _mockUsers.Setup(r => r.GetAllAsync())
                      .ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.GetAllUsersAsync());
        }

        // ---------------------------------------------------------
        // GET USER BY ID
        // ---------------------------------------------------------
        [Fact]
        public async Task GetUserById_ReturnsUser()
        {
            var user = new User { Id = "1", Username = "kostas" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _service.GetUserByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync((User)null);

            var result = await _service.GetUserByIdAsync("1");

            Assert.Null(result);
        }

        // ---------------------------------------------------------
        // PROMOTE TO ADMIN
        // ---------------------------------------------------------
        [Fact]
        public async Task PromoteToAdmin_Succeeds_WhenUserExists()
        {
            var user = new User { Id = "1", Role = "user" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var result = await _service.PromoteToAdminAsync("1");

            Assert.True(result);
            Assert.Equal("admin", user.Role);
        }

        [Fact]
        public async Task PromoteToAdmin_PromotesUser_WhenRoleIsNull()
        {
            var user = new User { Id = "1", Role = null };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var result = await _service.PromoteToAdminAsync("1");

            Assert.True(result);
            Assert.Equal("admin", user.Role);
        }

        [Fact]
        public async Task PromoteToAdmin_ReturnsTrue_WhenAlreadyAdmin()
        {
            var user = new User { Id = "1", Role = "admin" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _service.PromoteToAdminAsync("1");

            Assert.True(result);
            Assert.Equal("admin", user.Role);
        }

        [Fact]
        public async Task PromoteToAdmin_ReturnsFalse_WhenUserNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync((User)null);

            var result = await _service.PromoteToAdminAsync("1");

            Assert.False(result);
        }

        [Fact]
        public async Task PromoteToAdmin_ReturnsFalse_WhenUserIdIsInvalid()
        {
            var user = new User { Id = null, Role = "user" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _service.PromoteToAdminAsync("1");

            Assert.False(result);
        }

        [Fact]
        public async Task PromoteToAdmin_Throws_WhenRepositoryThrowsOnGet()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1"))
                      .ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.PromoteToAdminAsync("1"));
        }

        [Fact]
        public async Task PromoteToAdmin_Throws_WhenRepositoryThrowsOnUpdate()
        {
            var user = new User { Id = "1", Role = "user" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user))
                      .ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.PromoteToAdminAsync("1"));
        }

        // ---------------------------------------------------------
        // DELETE USER
        // ---------------------------------------------------------
        [Fact]
        public async Task DeleteUser_Succeeds_WhenRepositoryReturnsTrue()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.DeleteAsync("1")).ReturnsAsync(true);

            var result = await _service.DeleteUserAsync("1");

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsFalse_WhenRepositoryFails()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.DeleteAsync("1")).ReturnsAsync(false);

            var result = await _service.DeleteUserAsync("1");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsFalse_WhenUserNotFound()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync((User)null);

            var result = await _service.DeleteUserAsync("1");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsFalse_WhenUserIdIsInvalid()
        {
            var user = new User { Id = null };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _service.DeleteUserAsync("1");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUser_Throws_WhenRepositoryThrowsOnDelete()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.DeleteAsync("1"))
                      .ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteUserAsync("1"));
        }
    }
}