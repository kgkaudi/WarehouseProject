using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Service
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _users;
        private readonly IProductRepository _products;

        public UsersService(IUserRepository users, IProductRepository products)
        {
            _users = users;
            _products = products;
        }

        // ---------------------------------------------------------
        // GET USERS + PRODUCTS
        // ---------------------------------------------------------
        public async Task<IEnumerable<UserReadDto>> GetUsersAsync()
        {
            var users = await _users.GetAllAsync();
            if (users is null)
                return new List<UserReadDto>();

            var result = new List<UserReadDto>();

            foreach (var u in users)
            {
                var products = await _products.GetByUserIdAsync(u.Id);

                if (products is null)
                    throw new Exception($"Failed to load products for user {u.Id}");

                var safeProducts = products.Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Dimensions = p.Dimensions,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Weight = p.Weight
                }).ToList();

                result.Add(new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    CompanyName = u.CompanyName,
                    CompanyAddress = u.CompanyAddress,
                    Role = u.Role,
                    Products = safeProducts
                });
            }

            return result;
        }

        // ---------------------------------------------------------
        // UPDATE USER
        // ---------------------------------------------------------
        public async Task<bool> UpdateUserAsync(string id, UserUpdateDto dto)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.Username = dto.Username;
            user.CompanyName = dto.CompanyName;
            user.CompanyAddress = dto.CompanyAddress;

            await _users.UpdateAsync(user);
            return true;
        }

        // ---------------------------------------------------------
        // DELETE USER + PRODUCTS
        // ---------------------------------------------------------
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return false;

            await _products.DeleteByUserIdAsync(id);
            await _users.DeleteAsync(id);

            return true;
        }

        // ---------------------------------------------------------
        // PROMOTE USER
        // ---------------------------------------------------------
        public async Task<bool> PromoteToAdminAsync(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.Role = "admin";
            await _users.UpdateAsync(user);

            return true;
        }

        // ---------------------------------------------------------
        // DEMOTE USER
        // ---------------------------------------------------------
        public async Task<bool> DemoteToUserAsync(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.Role = "user";
            await _users.UpdateAsync(user);

            return true;
        }
    }
}