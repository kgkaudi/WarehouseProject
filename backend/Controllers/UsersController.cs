using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service;
        }

        // GET /api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            try
            {
                var result = await _service.GetUsersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT /api/users/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateDto dto)
        {
            try
            {
                var success = await _service.UpdateUserAsync(id, dto);
                return success ? Ok("User updated") : NotFound("User not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE /api/users/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var success = await _service.DeleteUserAsync(id);
                return success ? Ok("User deleted") : NotFound("User not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST /api/users/promote/{id}
        [Authorize(Roles = "admin")]
        [HttpPost("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            try
            {
                var success = await _service.PromoteToAdminAsync(id);
                return success ? Ok("User promoted to admin") : NotFound("User not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST /api/users/demote/{id}
        [Authorize(Roles = "admin")]
        [HttpPost("demote/{id}")]
        public async Task<IActionResult> DemoteToUser(string id)
        {
            try
            {
                var success = await _service.DemoteToUserAsync(id);
                return success ? Ok("User demoted to user") : NotFound("User not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}