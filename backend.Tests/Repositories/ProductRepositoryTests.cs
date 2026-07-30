using Xunit;
using MongoDB.Driver;
using backend.Repositories;
using backend.Models;
using backend.Service;
using backend.Tests.Shared;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace backend.Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Product> _collection;
        private readonly ProductRepository _repo;

        public ProductRepositoryTests()
        {
            _db = TestMongo.GetDatabase("WarehouseTestDb_ProductTests");

            // 🔥 Clear ALL collections before each test class
            foreach (var name in _db.ListCollectionNames().ToList())
            {
                _db.DropCollection(name);
            }

            _collection = _db.GetCollection<Product>("Products");

            var mongoService = new MockMongoService(_collection);
            _repo = new ProductRepository(mongoService);
        }

        // 🔥 Clear ALL collections AFTER each test
        public void Dispose()
        {
            foreach (var name in _db.ListCollectionNames().ToList())
            {
                _db.DropCollection(name);
            }
        }

        private Product ValidProduct(string id, string userId = null)
        {
            return new Product
            {
                Id = id,
                Name = "Test",
                Description = "Desc",
                Dimensions = "10x10",
                Price = 10,
                Quantity = 1,
                Weight = 1,
                UserId = userId
            };
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            await _collection.InsertManyAsync(new[]
            {
                ValidProduct("1"),
                ValidProduct("2")
            });

            var result = await _repo.GetAllAsync();

            Assert.Equal(2, result!.Count());
        }

        [Fact]
        public async Task GetAllAsync_EmptyList_ReturnsEmpty()
        {
            var result = await _repo.GetAllAsync();
            Assert.Empty(result!);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsProduct()
        {
            await _collection.InsertOneAsync(ValidProduct("p1"));

            var result = await _repo.GetByIdAsync("p1");

            Assert.NotNull(result);
            Assert.Equal("p1", result!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var result = await _repo.GetByIdAsync("missing");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsList()
        {
            await _collection.InsertManyAsync(new[]
            {
                ValidProduct("1", "user123"),
                ValidProduct("2", "user123")
            });

            var result = await _repo.GetByUserIdAsync("user123");

            Assert.Equal(2, result!.Count());
        }

        [Fact]
        public async Task GetByUserIdAsync_EmptyList_ReturnsEmpty()
        {
            var result = await _repo.GetByUserIdAsync("user123");
            Assert.Empty(result!);
        }

        [Fact]
        public async Task AddAsync_InsertsProduct()
        {
            var p = ValidProduct("p1");

            await _repo.AddAsync(p);

            var saved = await _collection.Find(x => x.Id == "p1").FirstOrDefaultAsync();
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task UpdateAsync_ReplacesProduct()
        {
            await _collection.InsertOneAsync(ValidProduct("p1"));

            var updated = ValidProduct("p1");
            updated.Name = "New";

            await _repo.UpdateAsync(updated);

            var saved = await _collection.Find(x => x.Id == "p1").FirstOrDefaultAsync();
            Assert.Equal("New", saved!.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesProduct()
        {
            await _collection.InsertOneAsync(ValidProduct("p1"));

            await _repo.DeleteAsync("p1");

            var exists = await _collection.Find(x => x.Id == "p1").AnyAsync();
            Assert.False(exists);
        }

        [Fact]
        public async Task DeleteByUserIdAsync_DeletesProducts()
        {
            await _collection.InsertManyAsync(new[]
            {
                ValidProduct("1", "user123"),
                ValidProduct("2", "user123")
            });

            await _repo.DeleteByUserIdAsync("user123");

            var exists = await _collection.Find(x => x.UserId == "user123").AnyAsync();
            Assert.False(exists);
        }

        private class MockMongoService : IMongoDbService
        {
            public IMongoCollection<Product> Products { get; }
            public IMongoCollection<User> Users => throw new NotImplementedException();

            public MockMongoService(IMongoCollection<Product> products)
            {
                Products = products;
            }
        }
    }
}
