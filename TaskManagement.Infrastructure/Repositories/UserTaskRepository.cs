using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class UserTaskRepository : GenericRepository<UserTask>, IUserTaskRepository
{
    public UserTaskRepository(TaskManagementContext context) : base(context) { }

    public async Task<UserTask?> GetByIdAsync(Guid userId, Guid taskId)
    {
        return await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId && x.TaskId == taskId);
    }

    public Task<List<TaskEntity>> GetTasksByUserAsync(Guid userId)
        => Where(x => x.UserId == userId)
          .Select(x => x.Task)
          .ToListAsync();
}
