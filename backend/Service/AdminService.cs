using backend.Models;
using backend.Repositories;

namespace backend.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _users;

        public AdminService(IUserRepository users)
        {
            _users = users;
        }

        public async Task<IEnumerable<User>?> GetAllUsersAsync()
        {
            return await _users.GetAllAsync();
        }


        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.GetByIdAsync(id);
        }

        public async Task<bool> PromoteToAdminAsync(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
                return false;

            if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
                return true;

            user.Role = "admin";
            await _users.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
                return false;

            return await _users.DeleteAsync(id);
        }
    }
}
