using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces.Services;

public interface ITaskService
{
    Task<GetTaskDto> GetTaskByIdAsync(Guid id);
    Task<GetTaskDto?> CreateTaskAsync(CreateTaskDto taskDto);
    Task UpdateTaskAsync(UpdateTaskDto taskDto);
    Task DeleteTaskAsync(Guid id);
    Task AssignUserToTaskAsync(Guid taskId, Guid userId);
    Task<IEnumerable<GetTaskDto>?> FilterTasksAsync(Guid userId, TaskFilterDto filter);
}
