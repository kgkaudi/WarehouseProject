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

        public Task<List<User>> GetAllAsync() =>
            _users.Find(_ => true).ToListAsync();

        public Task<User?> GetByIdAsync(string id) =>
            _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public Task<User?> GetByUsernameAsync(string username) =>
            _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public Task<User?> GetByEmailAsync(string email) =>
            _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public Task<bool> UsernameExistsAsync(string username) =>
            _users.Find(u => u.Username == username).AnyAsync();

        public Task<bool> EmailExistsAsync(string email) =>
            _users.Find(u => u.Email == email).AnyAsync();

        public Task<User?> GetByEmailVerificationTokenAsync(string token) =>
            _users.Find(u =>
                u.EmailVerificationToken == token &&
                u.EmailVerificationTokenExpires > DateTime.UtcNow
            ).FirstOrDefaultAsync();

        public Task<User?> GetByPasswordResetTokenAsync(string token) =>
            _users.Find(u =>
                u.PasswordResetToken == token &&
                u.PasswordResetTokenExpires > DateTime.UtcNow
            ).FirstOrDefaultAsync();

        public Task CreateAsync(User user) =>
            _users.InsertOneAsync(user);

        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        public Task DeleteAsync(string id) =>
            _users.DeleteOneAsync(u => u.Id == id);
    }
}
