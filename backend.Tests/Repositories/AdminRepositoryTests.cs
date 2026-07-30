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
    public class AdminRepositoryTests : IDisposable
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<User> _collection;
        private readonly AdminRepository _repo;

        public AdminRepositoryTests()
        {
            _db = TestMongo.GetDatabase("WarehouseTestDb_AdminTests");

            // 🔥 Clear ALL collections before each test class
            foreach (var name in _db.ListCollectionNames().ToList())
            {
                _db.DropCollection(name);
            }

            _collection = _db.GetCollection<User>("Users");

            var mongoService = new MockMongoService(_collection);
            _repo = new AdminRepository(mongoService);
        }

        // 🔥 Clear ALL collections AFTER each test
        public void Dispose()
        {
            foreach (var name in _db.ListCollectionNames().ToList())
            {
                _db.DropCollection(name);
            }
        }

        private User ValidUser(string id, string username)
        {
            return new User
            {
                Id = id,
                Username = username,
                Email = $"{username}@test.com",
                CompanyName = "C",
                CompanyAddress = "A",
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 1 }
            };
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsList()
        {
            await _collection.InsertManyAsync(new[]
            {
                ValidUser("1", "A"),
                ValidUser("2", "B")
            });

            var result = await _repo.GetAllUsersAsync();

            Assert.Equal(2, result!.Count);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsUser()
        {
            await _collection.InsertOneAsync(ValidUser("u1", "Test"));

            var result = await _repo.GetByIdAsync("u1");

            Assert.NotNull(result);
            Assert.Equal("u1", result!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var result = await _repo.GetByIdAsync("missing");
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser_ReturnsTrue()
        {
            await _collection.InsertOneAsync(ValidUser("u1", "Test"));

            var result = await _repo.DeleteAsync("u1");

            Assert.True(result);

            var exists = await _collection.Find(x => x.Id == "u1").AnyAsync();
            Assert.False(exists);
        }

        [Fact]
        public async Task DeleteAsync_NoUserDeleted_ReturnsFalse()
        {
            var result = await _repo.DeleteAsync("missing");
            Assert.False(result);
        }

        private class MockMongoService : IMongoDbService
        {
            public IMongoCollection<Product> Products => throw new NotImplementedException();
            public IMongoCollection<User> Users { get; }

            public MockMongoService(IMongoCollection<User> users)
            {
                Users = users;
            }
        }
    }
}
