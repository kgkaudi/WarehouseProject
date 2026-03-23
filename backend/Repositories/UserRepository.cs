using backend.Models;
using backend.Service;
using MongoDB.Driver;

namespace backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbService _mongo;

    public UserRepository(MongoDbService mongo)
    {
        _mongo = mongo;
    }

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _mongo.Users.Find(_ => true).ToListAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        await _mongo.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task AddAsync(User user) =>
        await _mongo.Users.InsertOneAsync(user);
}