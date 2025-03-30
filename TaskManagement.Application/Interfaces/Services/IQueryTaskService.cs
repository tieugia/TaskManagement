using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces.Services;

public interface IQueryTaskService
{
    Task<IEnumerable<TaskEntity>?> GetUserTasksAsync(Guid userId, TaskStatus? status, int page, int pageSize);
    Task<IEnumerable<TaskTimelineDto>?> GetTaskFullHistoryAsync(Guid taskId);
    Task<IEnumerable<User>?> GetTaskContributorsAsync(Guid taskId);
}
