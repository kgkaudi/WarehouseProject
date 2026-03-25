using backend.Models;
using backend.Service;
using MongoDB.Driver;

namespace backend.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IMongoCollection<User> _users;

        public AdminRepository(IMongoDbService db)
        {
            _users = db.Users;
        }

        public Task<List<User>> GetAllUsersAsync() =>
            _users.Find(_ => true).ToListAsync();

        public Task<User?> GetByIdAsync(string id) =>
            _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
