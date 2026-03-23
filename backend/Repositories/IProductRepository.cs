using System;
using backend.Models;

namespace backend.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByUserIdAsync(int userId);
    Task AddAsync(Product product);
    Task DeleteAsync(Product product);
    Task SaveAsync();
}