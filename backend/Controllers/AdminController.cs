using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Service;
using MongoDB.Driver;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]   // Only admins allowed
    public class AdminController : ControllerBase
    {

        private readonly MongoDbService _mongo;

        public AdminController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mongo.Users.Find(_ => true).ToListAsync();
            return Ok(users);
        }

        [HttpPut("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _mongo.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return NotFound("User not found");

            user.Role = "admin";
            await _mongo.Users.ReplaceOneAsync(u => u.Id == id, user);

            return Ok("User promoted to admin");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _mongo.Users.DeleteOneAsync(u => u.Id == id);
            if (result.DeletedCount == 0)
                return NotFound("User not found");

            return Ok("User deleted");
        }
    }
}
