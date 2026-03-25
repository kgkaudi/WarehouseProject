using backend.DTOs;
using backend.Repositories;
using backend.Models;

namespace backend.Service;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<Product> CreateProductForUser(string userId, ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Dimensions = dto.Dimensions,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Weight = dto.Weight,
            UserId = userId
        };

        await _products.AddAsync(product);

        return product;
    }

    public async Task<Product?> UpdateProduct(string userId, string productId, ProductUpdateDto dto)
    {
        var product = await _products.GetByIdAsync(productId);
        if (product == null || product.UserId != userId)
            return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Dimensions = dto.Dimensions;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.Weight = dto.Weight;

        await _products.UpdateAsync(product);
        return product;
    }

    public async Task<bool> DeleteProduct(string userId, string productId)
    {
        var product = await _products.GetByIdAsync(productId);
        if (product == null || product.UserId != userId)
            return false;

        await _products.DeleteAsync(productId);
        return true;
    }
}