using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Service;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET /api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();

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

        // PUT /api/admin/promote/{id}
        [HttpPut("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid user id");

                var success = await _adminService.PromoteToAdminAsync(id);

                if (!success)
                    return NotFound("User not found");

                return Ok("User promoted to admin");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE /api/admin/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid user id");

                var success = await _adminService.DeleteUserAsync(id);

                if (!success)
                    return NotFound("User not found or delete failed");

                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}