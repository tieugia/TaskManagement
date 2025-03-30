using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(TaskManagementContext context) : base(context) { }

    public async Task<bool> UsernameExistAsync(string username)
        => await Query().AnyAsync(u => u.Username == username);

    public async Task<User?> GetByUsernameAsync(string username) 
        => await Query().SingleOrDefaultAsync(u => u.Username == username);
}
