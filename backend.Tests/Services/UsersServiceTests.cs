using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Service;
using Moq;

namespace backend.Tests.Services
{
    public class UsersServiceTests
    {
        private readonly Mock<IUserRepository> _mockUsers;
        private readonly Mock<IProductRepository> _mockProducts;
        private readonly UsersService _service;

        public UsersServiceTests()
        {
            _mockUsers = new Mock<IUserRepository>();
            _mockProducts = new Mock<IProductRepository>();
            _service = new UsersService(_mockUsers.Object, _mockProducts.Object);
        }

        // ---------------------------------------------------------
        // GET USERS + PRODUCTS
        // ---------------------------------------------------------

        [Fact]
        public async Task GetUsers_ReturnsMappedUsers()
        {
            var users = new List<User>
            {
                new User { Id = "1", Username = "kostas", CompanyName = "A", CompanyAddress = "B", Role = "user" }
            };

            var products = new List<Product>
            {
                new Product { Id = "p1", Name = "Prod", Description = "Desc", Dimensions = "10x10", Price = 10, Quantity = 5, Weight = 1 }
            };

            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mockProducts.Setup(r => r.GetByUserIdAsync("1")).ReturnsAsync(products);

            var result = await _service.GetUsersAsync();

            Assert.Single(result);
            Assert.Single(result.First().Products);
            Assert.Equal("kostas", result.First().Username);
        }

        [Fact]
        public async Task GetUsers_RepositoryReturnsNull_ReturnsEmptyList()
        {
            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync((List<User>?)null);

            var result = await _service.GetUsersAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsers_ProductsNull_ThrowsException()
        {
            var users = new List<User> { new User { Id = "1" } };

            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mockProducts.Setup(r => r.GetByUserIdAsync("1")).ReturnsAsync((IEnumerable<Product>?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.GetUsersAsync());
        }

        [Fact]
        public async Task GetUsers_ProductsEmpty_ReturnsUserWithEmptyProducts()
        {
            var users = new List<User> { new User { Id = "1" } };

            _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mockProducts.Setup(r => r.GetByUserIdAsync("1")).ReturnsAsync(new List<Product>());

            var result = await _service.GetUsersAsync();

            Assert.Single(result);
            Assert.Empty(result.First().Products);
        }

        [Fact]
        public async Task GetUsers_RepositoryThrows_PropagatesException()
        {
            _mockUsers.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.GetUsersAsync());
        }

        // ---------------------------------------------------------
        // UPDATE USER
        // ---------------------------------------------------------

        [Fact]
        public async Task UpdateUser_Succeeds()
        {
            var user = new User { Id = "1", Username = "old" };

            var dto = new UserUpdateDto
            {
                Username = "new",
                CompanyName = "C",
                CompanyAddress = "D"
            };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var result = await _service.UpdateUserAsync("1", dto);

            Assert.True(result);
            Assert.Equal("new", user.Username);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ReturnsFalse()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

            var result = await _service.UpdateUserAsync("missing", new UserUpdateDto());

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUser_RepositoryThrows_PropagatesException()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.UpdateUserAsync("1", new UserUpdateDto()));
        }

        // ---------------------------------------------------------
        // DELETE USER + PRODUCTS
        // ---------------------------------------------------------

        [Fact]
        public async Task DeleteUser_Succeeds()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockProducts.Setup(r => r.DeleteByUserIdAsync("1")).Returns(Task.CompletedTask);
            _mockUsers.Setup(r => r.DeleteAsync("1")).ReturnsAsync(true);

            var result = await _service.DeleteUserAsync("1");

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsFalse()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

            var result = await _service.DeleteUserAsync("missing");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUser_DeleteProductsThrows_PropagatesException()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockProducts.Setup(r => r.DeleteByUserIdAsync("1")).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteUserAsync("1"));
        }

        [Fact]
        public async Task DeleteUser_DeleteUserThrows_PropagatesException()
        {
            var user = new User { Id = "1" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockProducts.Setup(r => r.DeleteByUserIdAsync("1")).Returns(Task.CompletedTask);
            _mockUsers.Setup(r => r.DeleteAsync("1")).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteUserAsync("1"));
        }

        // ---------------------------------------------------------
        // PROMOTE USER
        // ---------------------------------------------------------

        [Fact]
        public async Task PromoteToAdmin_Succeeds()
        {
            var user = new User { Id = "1", Role = "user" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var result = await _service.PromoteToAdminAsync("1");

            Assert.True(result);
            Assert.Equal("admin", user.Role);
        }

        [Fact]
        public async Task PromoteToAdmin_UserNotFound_ReturnsFalse()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

            var result = await _service.PromoteToAdminAsync("missing");

            Assert.False(result);
        }

        [Fact]
        public async Task PromoteToAdmin_RepositoryThrows_PropagatesException()
        {
            var user = new User { Id = "1", Role = "user" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.PromoteToAdminAsync("1"));
        }

        // ---------------------------------------------------------
        // DEMOTE USER
        // ---------------------------------------------------------

        [Fact]
        public async Task DemoteToUser_Succeeds()
        {
            var user = new User { Id = "1", Role = "admin" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var result = await _service.DemoteToUserAsync("1");

            Assert.True(result);
            Assert.Equal("user", user.Role);
        }

        [Fact]
        public async Task DemoteToUser_UserNotFound_ReturnsFalse()
        {
            _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

            var result = await _service.DemoteToUserAsync("missing");

            Assert.False(result);
        }

        [Fact]
        public async Task DemoteToUser_RepositoryThrows_PropagatesException()
        {
            var user = new User { Id = "1", Role = "admin" };

            _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _mockUsers.Setup(r => r.UpdateAsync(user)).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.DemoteToUserAsync("1"));
        }
    }
}