using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> UsernameExistAsync(string username);
    Task<User?> GetByUsernameAsync(string username);
}
