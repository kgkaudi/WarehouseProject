using Microsoft.AspNetCore.Mvc;

using backend.DTOs;
using backend.Repositories;


namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserRepository users, IProductRepository products) : ControllerBase
    {
        private readonly IUserRepository _users = users;
        private readonly IProductRepository _products = products;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _users.GetAllAsync();

            var result = new List<UserReadDto>();

            foreach (var u in users)
            {
                var products = await _products.GetByUserIdAsync(u.Id);

                result.Add(new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    CompanyName = u.CompanyName,
                    CompanyAddress = u.CompanyAddress,
                    Products = products.Select(p => new ProductReadDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Dimensions = p.Dimensions,
                        Price = p.Price,
                        Weight = p.Weight
                    }).ToList()
                });
            }

            return Ok(result);
        }
    }
}
