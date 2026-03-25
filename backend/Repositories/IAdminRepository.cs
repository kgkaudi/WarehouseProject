using backend.Models;

namespace backend.Repositories
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(string id);
        Task UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);
    }
}
