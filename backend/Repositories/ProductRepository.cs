using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseContext _context;

    public ProductRepository(WarehouseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetByUserIdAsync(int userId) =>
        await _context.Products.Where(p => p.UserId == userId).ToListAsync();

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await Task.CompletedTask;
    }

    public async Task SaveAsync() =>
        await _context.SaveChangesAsync();
}
