using System;
using backend.Models;

namespace backend.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetByUserIdAsync(string userId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(string id);
    Task DeleteByUserIdAsync(string id);
}
