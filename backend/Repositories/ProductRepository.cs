using backend.Models;
using backend.Service;
using MongoDB.Driver;

namespace backend.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoDbService _mongo;

    public ProductRepository(IMongoDbService mongo)
    {
        _mongo = mongo;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _mongo.Products.Find(_ => true).ToListAsync();

    public async Task<Product?> GetByIdAsync(string id) =>
        await _mongo.Products.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Product>> GetByUserIdAsync(string userId) =>
        await _mongo.Products.Find(p => p.UserId == userId).ToListAsync();

    public async Task AddAsync(Product product) =>
        await _mongo.Products.InsertOneAsync(product);

    public async Task UpdateAsync(Product product) =>
        await _mongo.Products.ReplaceOneAsync(p => p.Id == product.Id, product);

    public async Task DeleteAsync(string id) =>
        await _mongo.Products.DeleteOneAsync(p => p.Id == id);

    public async Task DeleteByUserIdAsync(string userId) =>
        await _mongo.Products.DeleteManyAsync(p => p.UserId == userId);
}
