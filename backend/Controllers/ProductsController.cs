using backend.DTOs;
using backend.Service;
using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IProductRepository _products;

        public ProductsController(IProductService service, IProductRepository products)
        {
            _service = service;
            _products = products;
        }

        // ---------------------------------------------------------
        // GET MY PRODUCTS
        // ---------------------------------------------------------
        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetMyProducts()
        {
            try
            {
                var userId = User.FindFirst("UserId")!.Value;

                var products = await _products.GetByUserIdAsync(userId);

                var dto = products.Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Dimensions = p.Dimensions,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Weight = p.Weight
                });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // CREATE PRODUCT
        // ---------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")!.Value;
                var product = await _service.CreateProductForUser(userId, dto);

                return Ok(new ProductReadDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Dimensions = product.Dimensions,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Weight = product.Weight
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // UPDATE PRODUCT
        // ---------------------------------------------------------
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, ProductUpdateDto dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")!.Value;

                var updated = await _service.UpdateProduct(userId, id, dto);
                if (updated == null)
                    return NotFound("Product not found or not yours");

                return Ok(new ProductReadDto
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Description = updated.Description,
                    Dimensions = updated.Dimensions,
                    Price = updated.Price,
                    Quantity = updated.Quantity,
                    Weight = updated.Weight
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // DELETE PRODUCT
        // ---------------------------------------------------------
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var userId = User.FindFirst("UserId")!.Value;

                var success = await _service.DeleteProduct(userId, id);
                if (!success)
                    return NotFound("Product not found or not yours");

                return Ok(new { message = "Product deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // GET ALL PRODUCTS
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _products.GetAllAsync();

                var dto = products.Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Dimensions = p.Dimensions,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Weight = p.Weight
                });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
