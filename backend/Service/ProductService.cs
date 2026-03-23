using backend.DTOs;
using backend.Repositories;
using backend.Models;

namespace backend.Service;

public class ProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<Product> CreateProductForUser(int userId, ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Dimensions = dto.Dimensions,
            Price = dto.Price,
            Weight = dto.Weight,
            UserId = userId
        };

        await _products.AddAsync(product);
        await _products.SaveAsync();

        return product;
    }

    public async Task<Product?> UpdateProduct(int userId, int productId, ProductUpdateDto dto)
    {
        var product = await _products.GetByIdAsync(productId);
        if (product == null || product.UserId != userId)
            return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Dimensions = dto.Dimensions;
        product.Price = (decimal)dto.Price;
        product.Weight = dto.Weight;

        await _products.SaveAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(int userId, int productId)
    {
        var product = await _products.GetByIdAsync(productId);
        if (product == null || product.UserId != userId)
            return false;

        await _products.DeleteAsync(product);
        await _products.SaveAsync();
        return true;
    }
}