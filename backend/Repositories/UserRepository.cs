using System;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly WarehouseContext _context;

    public UserRepository(WarehouseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.Include(u => u.Products).ToListAsync();

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.Include(u => u.Products)
                            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public async Task SaveAsync() =>
        await _context.SaveChangesAsync();
}