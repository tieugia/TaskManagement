using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Repositories;

public interface IUserTaskRepository : IGenericRepository<UserTask>
{
    Task<UserTask?> GetByIdAsync(Guid userId, Guid taskId);
    Task<IEnumerable<TaskEntity>> FilterTasksAsync(Guid userId, TaskFilterDto filter);
}
