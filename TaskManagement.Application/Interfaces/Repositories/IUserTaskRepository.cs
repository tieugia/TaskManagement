using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Repositories;

public interface IUserTaskRepository : IGenericRepository<UserTask>
{
    Task<List<TaskEntity>> GetTasksByUserAsync(Guid userId);
}
