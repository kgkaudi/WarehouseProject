using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using backend.Controllers;
using backend.Service;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend.Tests;

public class UsersControllerTests
{
    private readonly Mock<IUsersService> _mockService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockService = new Mock<IUsersService>();
        _controller = new UsersController(_mockService.Object);
    }

    private static UserReadDto MakeUser(string id = "1", string username = "kostas") => new()
    {
        Id = id,
        Username = username,
        CompanyName = "TestCo",
        CompanyAddress = "Address",
        Role = "user",
        Products = new List<ProductReadDto>()
    };

    // ---------------------------------------------------------
    // GET /api/users
    // ---------------------------------------------------------

    [Fact]
    public async Task GetUsers_ReturnsOk_WithUsersFromService()
    {
        var users = new List<UserReadDto> { MakeUser("1"), MakeUser("2", "admin") };

        _mockService.Setup(s => s.GetUsersAsync()).ReturnsAsync(users);

        var actionResult = await _controller.GetUsers();
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);

        var dto = Assert.IsAssignableFrom<IEnumerable<UserReadDto>>(ok.Value);
        Assert.Equal(2, new List<UserReadDto>(dto).Count);
    }

    [Fact]
    public async Task GetUsers_ReturnsOk_WithEmptyList()
    {
        _mockService.Setup(s => s.GetUsersAsync()).ReturnsAsync(new List<UserReadDto>());

        var actionResult = await _controller.GetUsers();
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);

        var dto = Assert.IsAssignableFrom<IEnumerable<UserReadDto>>(ok.Value);
        Assert.Empty(dto);
    }

    [Fact]
    public async Task GetUsers_ServiceThrows_Returns500()
    {
        _mockService.Setup(s => s.GetUsersAsync())
                    .ThrowsAsync(new Exception("Service error"));

        var actionResult = await _controller.GetUsers();
        var obj = Assert.IsType<ObjectResult>(actionResult.Result);

        Assert.Equal(500, obj.StatusCode);
        Assert.Equal("Service error", obj.Value);
    }

    [Fact]
    public async Task GetUsers_ServiceReturnsNull_ReturnsOkWithNullValue()
    {
        // Edge case: interface declares a non-nullable IEnumerable<UserReadDto>, but the
        // controller has no null-check of its own — if the service ever violates that
        // contract, the controller should still pass it through as Ok(null) rather than throw.
        _mockService.Setup(s => s.GetUsersAsync())
                    .ReturnsAsync((IEnumerable<UserReadDto>)null!);

        var actionResult = await _controller.GetUsers();
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);

        Assert.Null(ok.Value);
    }

    // ---------------------------------------------------------
    // PUT /{id}
    // ---------------------------------------------------------

    [Fact]
    public async Task UpdateUser_Success_ReturnsOk()
    {
        _mockService.Setup(s => s.UpdateUserAsync("1", It.IsAny<UserUpdateDto>()))
                    .ReturnsAsync(true);

        var result = await _controller.UpdateUser("1", new UserUpdateDto()) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User updated", result.Value);
    }

    [Fact]
    public async Task UpdateUser_NotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.UpdateUserAsync("missing", It.IsAny<UserUpdateDto>()))
                    .ReturnsAsync(false);

        var result = await _controller.UpdateUser("missing", new UserUpdateDto()) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    [Fact]
    public async Task UpdateUser_ServiceThrows_Returns500()
    {
        _mockService.Setup(s => s.UpdateUserAsync("1", It.IsAny<UserUpdateDto>()))
                    .ThrowsAsync(new Exception("Service error"));

        var result = await _controller.UpdateUser("1", new UserUpdateDto());

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
        Assert.Equal("Service error", obj.Value);
    }

    [Fact]
    public async Task UpdateUser_EmptyId_ReturnsNotFound()
    {
        // Edge case: controller does no id validation itself — an empty string id
        // is forwarded as-is, and the service's "not found" result drives the response.
        _mockService.Setup(s => s.UpdateUserAsync("", It.IsAny<UserUpdateDto>()))
                    .ReturnsAsync(false);

        var result = await _controller.UpdateUser("", new UserUpdateDto()) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    [Fact]
    public async Task UpdateUser_NullDto_ForwardsToServiceWithoutValidation()
    {
        // Edge case: controller has no null-check on the dto; it's forwarded as-is.
        _mockService.Setup(s => s.UpdateUserAsync("1", null!))
                    .ReturnsAsync(true);

        var result = await _controller.UpdateUser("1", null!) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User updated", result.Value);
        _mockService.Verify(s => s.UpdateUserAsync("1", null!), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_CallsServiceWithExactIdAndDto()
    {
        var dto = new UserUpdateDto { Username = "new-name" };

        _mockService.Setup(s => s.UpdateUserAsync("42", dto)).ReturnsAsync(true);

        await _controller.UpdateUser("42", dto);

        _mockService.Verify(s => s.UpdateUserAsync("42", dto), Times.Once);
    }

    // ---------------------------------------------------------
    // DELETE /{id}
    // ---------------------------------------------------------

    [Fact]
    public async Task DeleteUser_Success_ReturnsOk()
    {
        _mockService.Setup(s => s.DeleteUserAsync("1")).ReturnsAsync(true);

        var result = await _controller.DeleteUser("1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User deleted", result.Value);
    }

    [Fact]
    public async Task DeleteUser_NotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.DeleteUserAsync("missing")).ReturnsAsync(false);

        var result = await _controller.DeleteUser("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    [Fact]
    public async Task DeleteUser_ServiceThrows_Returns500()
    {
        _mockService.Setup(s => s.DeleteUserAsync("1"))
                    .ThrowsAsync(new Exception("Service error"));

        var result = await _controller.DeleteUser("1");

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
        Assert.Equal("Service error", obj.Value);
    }

    [Fact]
    public async Task DeleteUser_WhitespaceId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.DeleteUserAsync("   ")).ReturnsAsync(false);

        var result = await _controller.DeleteUser("   ") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    [Fact]
    public async Task DeleteUser_CallsServiceExactlyOnce()
    {
        _mockService.Setup(s => s.DeleteUserAsync("1")).ReturnsAsync(true);

        await _controller.DeleteUser("1");

        _mockService.Verify(s => s.DeleteUserAsync("1"), Times.Once);
    }

    // ---------------------------------------------------------
    // POST promote/{id}
    // ---------------------------------------------------------

    [Fact]
    public async Task PromoteToAdmin_Success_ReturnsOk()
    {
        _mockService.Setup(s => s.PromoteToAdminAsync("1")).ReturnsAsync(true);

        var result = await _controller.PromoteToAdmin("1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User promoted to admin", result.Value);
    }

    [Fact]
    public async Task PromoteToAdmin_NotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.PromoteToAdminAsync("missing")).ReturnsAsync(false);

        var result = await _controller.PromoteToAdmin("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    [Fact]
    public async Task PromoteToAdmin_ServiceThrows_Returns500()
    {
        _mockService.Setup(s => s.PromoteToAdminAsync("1"))
                    .ThrowsAsync(new Exception("Service error"));

        var result = await _controller.PromoteToAdmin("1");

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
        Assert.Equal("Service error", obj.Value);
    }

    [Fact]
    public async Task PromoteToAdmin_EmptyId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.PromoteToAdminAsync("")).ReturnsAsync(false);

        var result = await _controller.PromoteToAdmin("") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    // ---------------------------------------------------------
    // POST demote/{id}
    // ---------------------------------------------------------

    [Fact]
    public async Task DemoteToUser_Success_ReturnsOk()
    {
        _mockService.Setup(s => s.DemoteToUserAsync("1")).ReturnsAsync(true);

        var result = await _controller.DemoteToUser("1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User demoted to user", result.Value);
    }

    [Fact]
    public async Task DemoteToUser_NotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.DemoteToUserAsync("missing")).ReturnsAsync(false);

        var result = await _controller.DemoteToUser("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }

    [Fact]
    public async Task DemoteToUser_ServiceThrows_Returns500()
    {
        _mockService.Setup(s => s.DemoteToUserAsync("1"))
                    .ThrowsAsync(new Exception("Service error"));

        var result = await _controller.DemoteToUser("1");

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
        Assert.Equal("Service error", obj.Value);
    }

    [Fact]
    public async Task DemoteToUser_NullId_ForwardsToServiceAndReturnsNotFound()
    {
        // Edge case: string id parameters aren't validated by the controller,
        // so even a null id is passed straight through to the service.
        _mockService.Setup(s => s.DemoteToUserAsync(null!)).ReturnsAsync(false);

        var result = await _controller.DemoteToUser(null!) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("User not found", result.Value);
    }
}