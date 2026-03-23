using Microsoft.AspNetCore.Mvc;

using backend.DTOs;
using backend.Repositories;


namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _users;

        public UsersController(IUserRepository users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _users.GetAllAsync();

            return Ok(users.Select(u => new UserReadDto
            {
                Id = u.Id,
                Username = u.Username,
                CompanyName = u.CompanyName,
                CompanyAddress = u.CompanyAddress,
                Products = u.Products.Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Dimensions = p.Dimensions,
                    Price = (decimal)p.Price,
                    Weight = p.Weight
                }).ToList()
            }));
        }
    }
}
