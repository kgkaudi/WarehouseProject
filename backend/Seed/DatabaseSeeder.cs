using backend.Models;
using backend.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace backend.Seed
{
    public class DatabaseSeeder
    {
        private readonly IUserRepository _users;
        private readonly IProductRepository _products;

        public DatabaseSeeder(IUserRepository users, IProductRepository products)
        {
            _users = users;
            _products = products;
        }

        public async Task SeedAsync()
        {
            // -----------------------------
            // 1. Seed Admin User
            // -----------------------------
            var admin = await _users.GetByEmailAsync("admin@test.com");

            if (admin == null)
            {
                CreatePasswordHash("Ectoras01!", out byte[] hash, out byte[] salt);

                admin = new User
                {
                    Username = "admin",
                    Email = "admin@test.com",
                    CompanyName = "Admin Company",
                    CompanyAddress = "Admin Street 1",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Role = "admin",
                    EmailConfirmed = true
                };

                await _users.CreateAsync(admin);
            }

            // -----------------------------
            // 2. Seed Test Product
            // -----------------------------
            var existingProduct = (await _products.GetByUserIdAsync(admin.Id)).FirstOrDefault();

            if (existingProduct == null)
            {
                var product = new Product
                {
                    UserId = admin.Id,
                    Name = "Test Product",
                    Description = "This is a seeded test product",
                    Dimensions = "10x10",
                    Price = 99.99,
                    Quantity = 10,
                    Weight = 1.5
                };

                await _products.AddAsync(product);
            }
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
