using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Repositories;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _users;

        public AdminController(IUserRepository users)
        {
            _users = users;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _users.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("promote/{id}")]
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                await _users.DeleteAsync(id);
                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
