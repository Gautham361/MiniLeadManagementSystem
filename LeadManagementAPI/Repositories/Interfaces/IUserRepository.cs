
using LeadManagementAPI.Models;

namespace LeadManagementAPI.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);

}
