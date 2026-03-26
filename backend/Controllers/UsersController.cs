using Microsoft.AspNetCore.Mvc;

using backend.DTOs;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;


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
                    Role = u.Role,
                    Products = products.Select(p => new ProductReadDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Dimensions = p.Dimensions,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Weight = p.Weight
                    }).ToList()
                });
            }

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateDto dto)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.Username = dto.Username;
            user.CompanyName = dto.CompanyName;
            user.CompanyAddress = dto.CompanyAddress;

            await _users.UpdateAsync(user);

            return Ok("User updated");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            // Delete all products belonging to this user
            await _products.DeleteByUserIdAsync(id);

            // Delete user
            await _users.DeleteAsync(id);

            return Ok("User deleted");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.Role = "admin";
            await _users.UpdateAsync(user);

            return Ok("User promoted to admin");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("demote/{id}")]
        public async Task<IActionResult> DemoteToUser(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.Role = "user";
            await _users.UpdateAsync(user);

            return Ok("User demoted to user");
        }
    }
}
