using backend.Models;
using backend.Service;
using MongoDB.Driver;

namespace backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDbService db)
        {
            _users = db.Users;
        }

        public async Task<List<User>?> GetAllAsync()
        {
            var list = await _users.Find(_ => true).ToListAsync();
            return list; // never null, but nullable return type matches interface
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return user; // may be null → matches interface
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public Task<bool> UsernameExistsAsync(string username) =>
            _users.Find(u => u.Username == username).AnyAsync();

        public Task<bool> EmailExistsAsync(string email) =>
            _users.Find(u => u.Email == email).AnyAsync();

        public async Task<User?> GetByEmailVerificationTokenAsync(string token)
        {
            var user = await _users.Find(u =>
                u.EmailVerificationToken == token &&
                u.EmailVerificationTokenExpires > DateTime.UtcNow
            ).FirstOrDefaultAsync();

            return user;
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            var user = await _users.Find(u =>
                u.PasswordResetToken == token &&
                u.PasswordResetTokenExpires > DateTime.UtcNow
            ).FirstOrDefaultAsync();

            return user;
        }

        public Task CreateAsync(User user) =>
            _users.InsertOneAsync(user);

        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        public Task DeleteAsync(string id) =>
            _users.DeleteOneAsync(u => u.Id == id);
    }
}