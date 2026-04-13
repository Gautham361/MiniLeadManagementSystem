using LeadManagementAPI.Data;
using LeadManagementAPI.Models;
using LeadManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);


}
