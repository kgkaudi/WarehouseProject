using backend.Models;

namespace backend.Service
{
    public interface IAdminService
    {
        Task<IEnumerable<User>?> GetAllUsersAsync();   // add the `?`
        Task<User?> GetUserByIdAsync(string id);
        Task<bool> PromoteToAdminAsync(string id);
        Task<bool> DeleteUserAsync(string id);
    }
}