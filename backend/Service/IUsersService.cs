using backend.DTOs;

namespace backend.Service
{
    public interface IUsersService
    {
        Task<IEnumerable<UserReadDto>> GetUsersAsync();
        Task<bool> UpdateUserAsync(string id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> PromoteToAdminAsync(string id);
        Task<bool> DemoteToUserAsync(string id);
    }
}
