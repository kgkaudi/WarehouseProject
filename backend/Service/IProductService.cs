using System;
using backend.DTOs;
using backend.Models;

namespace backend.Service;

public interface IProductService
{
    Task<Product> CreateProductForUser(string userId, ProductCreateDto dto);
    Task<Product?> UpdateProduct(string userId, string id, ProductUpdateDto dto);
    Task<bool> DeleteProduct(string userId, string id);
}
