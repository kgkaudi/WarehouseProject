using System;
using Xunit;
using Moq;
using backend.Controllers;
using backend.Service;
using backend.Repositories;
using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace backend.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _mockRepo = new Mock<IProductRepository>();

        _controller = new ProductsController(_mockService.Object, _mockRepo.Object);
    }

    private void SetUser(string userId)
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserId", userId)
                }))
            }
        };
    }

    // ---------------------------------------------------------
    // GET /mine
    // ---------------------------------------------------------

    [Fact]
    public async Task GetMyProducts_ReturnsProductsForUser()
    {
        SetUser("user123");

        var products = new List<Product>
        {
            new Product { Id = "1", Name = "A", UserId = "user123" },
            new Product { Id = "2", Name = "B", UserId = "user123" }
        };

        _mockRepo.Setup(r => r.GetByUserIdAsync("user123"))
                 .ReturnsAsync(products);

        var actionResult = await _controller.GetMyProducts();
        var okResult = actionResult.Result as OkObjectResult;

        Assert.NotNull(okResult);
        var dto = Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(okResult.Value);
        Assert.Equal(2, dto.Count());
    }

    [Fact]
    public async Task GetMyProducts_RepositoryThrows_Returns500()
    {
        SetUser("user123");

        _mockRepo.Setup(r => r.GetByUserIdAsync("user123"))
                 .ThrowsAsync(new Exception("DB error"));

        var result = await _controller.GetMyProducts();

        var obj = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, obj.StatusCode);
    }

    // ---------------------------------------------------------
    // POST /
    // ---------------------------------------------------------

    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        SetUser("user123");

        var dto = new ProductCreateDto
        {
            Name = "Test",
            Description = "Desc",
            Price = 10,
            Quantity = 5,
            Dimensions = "10x10",
            Weight = 1
        };

        var created = new Product
        {
            Id = "p1",
            Name = "Test",
            Description = "Desc",
            Price = 10,
            Quantity = 5,
            Dimensions = "10x10",
            Weight = 1
        };

        _mockService.Setup(s => s.CreateProductForUser("user123", dto))
                    .ReturnsAsync(created);

        var result = await _controller.CreateProduct(dto) as OkObjectResult;

        Assert.NotNull(result);
        var productDto = Assert.IsType<ProductReadDto>(result.Value);
        Assert.Equal("p1", productDto.Id);
    }

    [Fact]
    public async Task CreateProduct_ServiceThrows_Returns500()
    {
        SetUser("user123");

        var dto = new ProductCreateDto { Name = "X" };

        _mockService.Setup(s => s.CreateProductForUser("user123", dto))
                    .ThrowsAsync(new Exception("DB error"));

        var result = await _controller.CreateProduct(dto);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    // ---------------------------------------------------------
    // PUT /{id}
    // ---------------------------------------------------------

    [Fact]
    public async Task UpdateProduct_ProductExists_ReturnsOk()
    {
        SetUser("user123");

        var dto = new ProductUpdateDto { Name = "Updated" };

        var updated = new Product
        {
            Id = "p1",
            Name = "Updated",
            UserId = "user123"
        };

        _mockService.Setup(s => s.UpdateProduct("user123", "p1", dto))
                    .ReturnsAsync(updated);

        var result = await _controller.UpdateProduct("p1", dto) as OkObjectResult;

        Assert.NotNull(result);
        var productDto = Assert.IsType<ProductReadDto>(result.Value);
        Assert.Equal("Updated", productDto.Name);
    }

    [Fact]
    public async Task UpdateProduct_NotFound_ReturnsNotFound()
    {
        SetUser("user123");

        var dto = new ProductUpdateDto { Name = "Updated" };

        _mockService.Setup(s => s.UpdateProduct("user123", "missing", dto))
                    .ReturnsAsync((Product)null);

        var result = await _controller.UpdateProduct("missing", dto) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("Product not found or not yours", result.Value);
    }

    [Fact]
    public async Task UpdateProduct_ServiceThrows_Returns500()
    {
        SetUser("user123");

        var dto = new ProductUpdateDto { Name = "Updated" };

        _mockService.Setup(s => s.UpdateProduct("user123", "p1", dto))
                    .ThrowsAsync(new Exception("DB error"));

        var result = await _controller.UpdateProduct("p1", dto);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    // ---------------------------------------------------------
    // DELETE /{id}
    // ---------------------------------------------------------

    [Fact]
    public async Task DeleteProduct_Success_ReturnsOk()
    {
        SetUser("user123");

        _mockService.Setup(s => s.DeleteProduct("user123", "p1"))
                    .ReturnsAsync(true);

        var result = await _controller.DeleteProduct("p1") as OkObjectResult;

        Assert.NotNull(result);

        var dict = result.Value.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(result.Value));

        Assert.Equal("Product deleted", dict["message"]);
    }

    [Fact]
    public async Task DeleteProduct_NotFound_ReturnsNotFound()
    {
        SetUser("user123");

        _mockService.Setup(s => s.DeleteProduct("user123", "missing"))
                    .ReturnsAsync(false);

        var result = await _controller.DeleteProduct("missing") as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal("Product not found or not yours", result.Value);
    }

    [Fact]
    public async Task DeleteProduct_ServiceThrows_Returns500()
    {
        SetUser("user123");

        _mockService.Setup(s => s.DeleteProduct("user123", "p1"))
                    .ThrowsAsync(new Exception("DB error"));

        var result = await _controller.DeleteProduct("p1");

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    // ---------------------------------------------------------
    // GET /
    // ---------------------------------------------------------

    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = "1", Name = "A" },
            new Product { Id = "2", Name = "B" }
        };

        _mockRepo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(products);

        var result = await _controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(result.Value);
        Assert.Equal(2, dto.Count());
    }

    [Fact]
    public async Task GetAll_RepositoryThrows_Returns500()
    {
        _mockRepo.Setup(r => r.GetAllAsync())
                 .ThrowsAsync(new Exception("DB error"));

        var result = await _controller.GetAll();

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }
}
