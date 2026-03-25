using System;
using Xunit;
using Moq;
using backend.Controllers;
using backend.Repositories;
using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Tests;

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _mockUsers;
    private readonly Mock<IProductRepository> _mockProducts;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUsers = new Mock<IUserRepository>();
        _mockProducts = new Mock<IProductRepository>();

        _controller = new UsersController(_mockUsers.Object, _mockProducts.Object);
    }

    // ---------------------------
    // GET /api/users
    // ---------------------------
    [Fact]
    public async Task GetUsers_ReturnsUsersWithProducts()
    {
        var users = new List<User>
        {
            new User { Id = "1", Username = "A", CompanyName = "C1" },
            new User { Id = "2", Username = "B", CompanyName = "C2" }
        };

        var productsUser1 = new List<Product>
        {
            new Product { Id = "p1", Name = "Prod1", UserId = "1" }
        };

        var productsUser2 = new List<Product>
        {
            new Product { Id = "p2", Name = "Prod2", UserId = "2" }
        };

        _mockUsers.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
        _mockProducts.Setup(r => r.GetByUserIdAsync("1")).ReturnsAsync(productsUser1);
        _mockProducts.Setup(r => r.GetByUserIdAsync("2")).ReturnsAsync(productsUser2);

        var actionResult = await _controller.GetUsers();
        var okResult = actionResult.Result as OkObjectResult;

        Assert.NotNull(okResult);
        var dto = Assert.IsAssignableFrom<IEnumerable<UserReadDto>>(okResult.Value);

        Assert.Equal(2, dto.Count());
    }

    // ---------------------------
    // PUT /{id}
    // ---------------------------
    [Fact]
    public async Task UpdateUser_UserExists_ReturnsOk()
    {
        var user = new User { Id = "1", Username = "Old" };

        _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var dto = new UserUpdateDto
        {
            Username = "New",
            CompanyName = "NewCo",
            CompanyAddress = "NewAddr"
        };

        var result = await _controller.UpdateUser("1", dto) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User updated", result.Value);
        Assert.Equal("New", user.Username);
    }

    [Fact]
    public async Task UpdateUser_NotFound_ReturnsNotFound()
    {
        _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User)null);

        var dto = new UserUpdateDto();

        var result = await _controller.UpdateUser("missing", dto) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    // ---------------------------
    // DELETE /{id}
    // ---------------------------
    [Fact]
    public async Task DeleteUser_UserExists_DeletesUserAndProducts()
    {
        var user = new User { Id = "1" };

        _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var result = await _controller.DeleteUser("1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User deleted", result.Value);

        _mockProducts.Verify(r => r.DeleteByUserIdAsync("1"), Times.Once);
        _mockUsers.Verify(r => r.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_NotFound_ReturnsNotFound()
    {
        _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User)null);

        var result = await _controller.DeleteUser("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    // ---------------------------
    // POST promote/{id}
    // ---------------------------
    [Fact]
    public async Task PromoteToAdmin_UserExists_ReturnsOk()
    {
        var user = new User { Id = "1", Role = "user" };

        _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var result = await _controller.PromoteToAdmin("1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User promoted to admin", result.Value);
        Assert.Equal("admin", user.Role);
    }

    [Fact]
    public async Task PromoteToAdmin_NotFound_ReturnsNotFound()
    {
        _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User)null);

        var result = await _controller.PromoteToAdmin("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    // ---------------------------
    // POST demote/{id}
    // ---------------------------
    [Fact]
    public async Task DemoteToUser_UserExists_ReturnsOk()
    {
        var user = new User { Id = "1", Role = "admin" };

        _mockUsers.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var result = await _controller.DemoteToUser("1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User demoted to user", result.Value);
        Assert.Equal("user", user.Role);
    }

    [Fact]
    public async Task DemoteToUser_NotFound_ReturnsNotFound()
    {
        _mockUsers.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User)null);

        var result = await _controller.DemoteToUser("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }
}
