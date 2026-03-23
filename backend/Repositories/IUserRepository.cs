using System;
using backend.Models;

namespace backend.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string id);
}