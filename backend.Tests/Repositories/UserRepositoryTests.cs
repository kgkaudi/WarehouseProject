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
    public class UserRepositoryTests : IDisposable
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<User> _collection;
        private readonly UserRepository _repo;

        public UserRepositoryTests()
        {
            _db = TestMongo.GetDatabase("WarehouseTestDb_UserTests");

            // 🔥 Clear ALL collections before each test class
            foreach (var name in _db.ListCollectionNames().ToList())
            {
                _db.DropCollection(name);
            }

            _collection = _db.GetCollection<User>("Users");

            var mongoService = new MockMongoService(_collection);
            _repo = new UserRepository(mongoService);
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
        public async Task GetAllAsync_ReturnsList()
        {
            await _collection.InsertManyAsync(new[]
            {
                ValidUser("1", "A"),
                ValidUser("2", "B")
            });

            var result = await _repo.GetAllAsync();

            Assert.Equal(2, result!.Count);
        }

        [Fact]
        public async Task GetAllAsync_EmptyList_ReturnsEmpty()
        {
            var result = await _repo.GetAllAsync();
            Assert.Empty(result!);
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
        public async Task GetByUsernameAsync_Found_ReturnsUser()
        {
            await _collection.InsertOneAsync(ValidUser("u1", "kostas"));

            var result = await _repo.GetByUsernameAsync("kostas");

            Assert.NotNull(result);
            Assert.Equal("kostas", result!.Username);
        }

        [Fact]
        public async Task GetByUsernameAsync_NotFound_ReturnsNull()
        {
            var result = await _repo.GetByUsernameAsync("missing");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_Found_ReturnsUser()
        {
            var user = ValidUser("u1", "test");
            user.Email = "test@test.com";

            await _collection.InsertOneAsync(user);

            var result = await _repo.GetByEmailAsync("test@test.com");

            Assert.NotNull(result);
            Assert.Equal("test@test.com", result!.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_NotFound_ReturnsNull()
        {
            var result = await _repo.GetByEmailAsync("missing@test.com");
            Assert.Null(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_ReturnsTrue()
        {
            await _collection.InsertOneAsync(ValidUser("u1", "kostas"));

            var result = await _repo.UsernameExistsAsync("kostas");

            Assert.True(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_ReturnsFalse()
        {
            var result = await _repo.UsernameExistsAsync("missing");
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ReturnsTrue()
        {
            var user = ValidUser("u1", "test");
            user.Email = "test@test.com";

            await _collection.InsertOneAsync(user);

            var result = await _repo.EmailExistsAsync("test@test.com");

            Assert.True(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ReturnsFalse()
        {
            var result = await _repo.EmailExistsAsync("missing@test.com");
            Assert.False(result);
        }

        [Fact]
        public async Task GetByEmailVerificationTokenAsync_ValidToken_ReturnsUser()
        {
            var user = ValidUser("u1", "test");
            user.EmailVerificationToken = "token123";
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(10);

            await _collection.InsertOneAsync(user);

            var result = await _repo.GetByEmailVerificationTokenAsync("token123");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByEmailVerificationTokenAsync_ExpiredToken_ReturnsNull()
        {
            var user = ValidUser("u1", "test");
            user.EmailVerificationToken = "token123";
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(-10);

            await _collection.InsertOneAsync(user);

            var result = await _repo.GetByEmailVerificationTokenAsync("token123");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByPasswordResetTokenAsync_ValidToken_ReturnsUser()
        {
            var user = ValidUser("u1", "test");
            user.PasswordResetToken = "reset123";
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddMinutes(10);

            await _collection.InsertOneAsync(user);

            var result = await _repo.GetByPasswordResetTokenAsync("reset123");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByPasswordResetTokenAsync_ExpiredToken_ReturnsNull()
        {
            var user = ValidUser("u1", "test");
            user.PasswordResetToken = "reset123";
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddMinutes(-10);

            await _collection.InsertOneAsync(user);

            var result = await _repo.GetByPasswordResetTokenAsync("reset123");

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_InsertsUser()
        {
            var user = ValidUser("u1", "Test");

            await _repo.CreateAsync(user);

            var saved = await _collection.Find(x => x.Id == "u1").FirstOrDefaultAsync();
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task UpdateAsync_ReplacesUser()
        {
            await _collection.InsertOneAsync(ValidUser("u1", "Old"));

            var updated = ValidUser("u1", "New");

            await _repo.UpdateAsync(updated);

            var saved = await _collection.Find(x => x.Id == "u1").FirstOrDefaultAsync();
            Assert.Equal("New", saved!.Username);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser()
        {
            await _collection.InsertOneAsync(ValidUser("u1", "Test"));

            await _repo.DeleteAsync("u1");

            var exists = await _collection.Find(x => x.Id == "u1").AnyAsync();
            Assert.False(exists);
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
