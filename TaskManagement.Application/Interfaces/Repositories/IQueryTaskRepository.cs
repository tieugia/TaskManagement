using TaskManagement.Domain.Entities;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces.Repositories;

public interface IQueryTaskRepository
{
    Task<IEnumerable<TaskEntity>> GetUserTasksAsync(Guid userId, TaskStatus? status, int page, int pageSize);
    Task<IEnumerable<TaskTimeline>> GetTaskFullHistoryAsync(Guid taskId);
    Task<IEnumerable<User>> GetTaskContributorsAsync(Guid taskId);
}
