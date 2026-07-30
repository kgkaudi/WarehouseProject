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

        public async Task<List<User>?> GetAllUsersAsync()
        {
            var list = await _users.Find(_ => true).ToListAsync();
            return list; // list is never null, but nullable return type is allowed
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return user; // may be null → matches interface
        }

        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
