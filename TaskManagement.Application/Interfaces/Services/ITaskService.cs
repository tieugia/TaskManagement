using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Services;

public interface ITaskService
{
    Task<GetTaskDto> GetTaskByIdAsync(Guid id);
    Task<GetTaskDto?> CreateTaskAsync(CreateTaskDto taskDto);
    Task UpdateTaskAsync(UpdateTaskDto taskDto);
    Task DeleteTaskAsync(Guid id);
    Task<List<TaskEntity>> GetTasksByUserAsync(Guid userId);
}
