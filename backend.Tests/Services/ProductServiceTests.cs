using Xunit;
using Moq;
using backend.Service;
using backend.Repositories;
using backend.Models;
using backend.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace backend.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepo.Object);
        }

        // ---------------------------------------------------------
        // CREATE PRODUCT
        // ---------------------------------------------------------
        [Fact]
        public async Task CreateProductForUser_ValidDto_ReturnsProduct()
        {
            var dto = new ProductCreateDto
            {
                Name = "Test",
                Description = "Desc",
                Dimensions = "10x10",
                Price = 10,
                Quantity = 5,
                Weight = 1
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
                     .Returns(Task.CompletedTask);

            var result = await _service.CreateProductForUser("user123", dto);

            Assert.NotNull(result);
            Assert.Equal("user123", result.UserId);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task CreateProductForUser_NullDto_ThrowsException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _service.CreateProductForUser("user123", null!));
        }

        // ---------------------------------------------------------
        // UPDATE PRODUCT
        // ---------------------------------------------------------
        [Fact]
        public async Task UpdateProduct_ValidUserAndProduct_ReturnsUpdatedProduct()
        {
            var existing = new Product { Id = "p1", UserId = "user123", Name = "Old" };
            var dto = new ProductUpdateDto { Name = "New" };

            _mockRepo.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateProduct("user123", "p1", dto);

            Assert.NotNull(result);
            Assert.Equal("New", result!.Name);
        }

        [Fact]
        public async Task UpdateProduct_ProductNotFound_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((Product?)null);

            var dto = new ProductUpdateDto { Name = "Updated" };
            var result = await _service.UpdateProduct("user123", "missing", dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProduct_NotOwnedByUser_ReturnsNull()
        {
            var existing = new Product { Id = "p1", UserId = "otherUser" };
            var dto = new ProductUpdateDto { Name = "Updated" };

            _mockRepo.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(existing);

            var result = await _service.UpdateProduct("user123", "p1", dto);

            Assert.Null(result);
        }

        // ---------------------------------------------------------
        // DELETE PRODUCT
        // ---------------------------------------------------------
        [Fact]
        public async Task DeleteProduct_ValidUserAndProduct_ReturnsTrue()
        {
            var existing = new Product { Id = "p1", UserId = "user123" };

            _mockRepo.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.DeleteAsync("p1")).Returns(Task.CompletedTask);

            var result = await _service.DeleteProduct("user123", "p1");

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProduct_ProductNotFound_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((Product?)null);

            var result = await _service.DeleteProduct("user123", "missing");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProduct_NotOwnedByUser_ReturnsFalse()
        {
            var existing = new Product { Id = "p1", UserId = "otherUser" };
            _mockRepo.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(existing);

            var result = await _service.DeleteProduct("user123", "p1");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProduct_RepositoryThrows_ReturnsException()
        {
            var existing = new Product { Id = "p1", UserId = "user123" };
            _mockRepo.Setup(r => r.GetByIdAsync("p1")).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.DeleteAsync("p1")).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteProduct("user123", "p1"));
        }
    }
}