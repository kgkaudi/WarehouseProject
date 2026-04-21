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

        // ---------------------------------------------------------
        // GET ALL USERS + THEIR PRODUCTS
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // UPDATE USER
        // ---------------------------------------------------------
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateDto dto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // DELETE USER + THEIR PRODUCTS
        // ---------------------------------------------------------
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                await _products.DeleteByUserIdAsync(id);
                await _users.DeleteAsync(id);

                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // PROMOTE USER
        // ---------------------------------------------------------
        [Authorize(Roles = "admin")]
        [HttpPost("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                user.Role = "admin";
                await _users.UpdateAsync(user);

                return Ok("User promoted to admin");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // DEMOTE USER
        // ---------------------------------------------------------
        [Authorize(Roles = "admin")]
        [HttpPost("demote/{id}")]
        public async Task<IActionResult> DemoteToUser(string id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                user.Role = "user";
                await _users.UpdateAsync(user);

                return Ok("User demoted to user");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
