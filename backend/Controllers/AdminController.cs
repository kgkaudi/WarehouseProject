using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Repositories;
using backend.Models;

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

        // ---------------------------------------------------------
        // GET /api/admin/users
        // ---------------------------------------------------------
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _users.GetAllAsync();

                if (users == null)
                    return StatusCode(500, "Repository returned null");

                if (users.Any(u => u == null))
                    return StatusCode(500, "Repository returned invalid user entries");

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ---------------------------------------------------------
        // PUT /api/admin/promote/{id}
        // ---------------------------------------------------------
        [HttpPut("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid user id");

                var user = await _users.GetByIdAsync(id);

                if (user == null)
                    return NotFound("User not found");

                if (string.IsNullOrWhiteSpace(user.Id))
                    return StatusCode(500, "Repository returned invalid user");

                if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
                    return Ok("User is already admin");

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
        // DELETE /api/admin/delete/{id}
        // ---------------------------------------------------------
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid user id");

                var user = await _users.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                if (string.IsNullOrWhiteSpace(user.Id))
                    return StatusCode(500, "Repository returned invalid user");

                var deleted = await _users.DeleteAsync(id);

                if (!deleted)
                    return StatusCode(500, "Delete failed");

                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}