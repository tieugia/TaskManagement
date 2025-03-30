using System.Threading.Tasks;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Common.Constants;
using TaskManagement.Common.Helpers;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly ICacheService _cacheService;

    public TaskService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUserTaskRepository userTaskRepository,
        ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _userTaskRepository = userTaskRepository;
        _cacheService = cacheService;
    }

    public async Task<GetTaskDto> GetTaskByIdAsync(Guid id)
    {
        var cacheKey = CacheKeys.Task(id);
        var cache = await _cacheService.GetAsync<GetTaskDto>(cacheKey);
        if (cache != null) return cache;

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        var result = new GetTaskDto
        {
            Id = task.Id,
            Status = task.Status,
            Title = task.Title,
            Description = task.Description,
            CreatedBy = task.CreatedBy,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            RowVersion = task.RowVersion
        };

        await _cacheService.SetAsync(cacheKey, result);
        return result;
    }

    public async Task<GetTaskDto?> CreateTaskAsync(CreateTaskDto taskDto)
    {
        var task = new TaskEntity
        {
            Title = taskDto.Title,
            Status = taskDto.Status,
            Description = taskDto.Description,
            CreatedBy = taskDto.CreatedBy,
            CreatedAt = taskDto.CreatedAt
        };

        var createdTask = await _taskRepository.AddAsync(task);

        if (createdTask == null)
            return null;

        await _cacheService.RemoveAsync(CacheKeys.TasksByUser(task.CreatedBy));

        return new GetTaskDto
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Status = createdTask.Status,
            Description = createdTask.Description,
            CreatedAt = createdTask.CreatedAt,
            UpdatedAt = createdTask.UpdatedAt,
            RowVersion = createdTask.RowVersion
        };
    }

    public async Task UpdateTaskAsync(UpdateTaskDto taskDto)
    {
        var task = await _taskRepository.GetByIdAsync(taskDto.Id);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        task.Title = taskDto.Title ?? task.Title;
        task.Description = taskDto.Description;
        task.Status = taskDto.Status;
        task.UpdatedAt = taskDto.UpdatedAt;
        task.RowVersion = taskDto.RowVersion;

        await _taskRepository.UpdateAsync(task);

        await _cacheService.RemoveAsync(CacheKeys.Task(taskDto.Id));
        await _cacheService.RemoveAsync(CacheKeys.TasksByUser(task.CreatedBy));
    }

    public async Task DeleteTaskAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        await _taskRepository.DeleteAsync(task);

        await _cacheService.RemoveAsync(CacheKeys.Task(id));
        await _cacheService.RemoveAsync(CacheKeys.TasksByUser(task.CreatedBy));
    }

    public async Task<IEnumerable<GetTaskDto>?> FilterTasksAsync(Guid userId, TaskFilterDto filter)
    {
        var cacheKey = CacheKeys.TasksByUser(userId);
        var cached = await _cacheService.GetAsync<List<GetTaskDto>>(cacheKey);
        if (cached != null) return cached;

        var userExists = await _userRepository.ExistAsync(userId);
        if (!userExists)
            throw new KeyNotFoundException("User not found");

        var tasks = await _userTaskRepository.FilterTasksAsync(userId, filter);

        if (tasks.IsNullOrEmpty())
            return null;

        var taskDtos = tasks.Select(t => new GetTaskDto
        {
            Id = t.Id,
            Status = t.Status,
            Title = t.Title,
            Description = t.Description,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            RowVersion = t.RowVersion
        });

        await _cacheService.SetAsync(cacheKey, taskDtos);

        return taskDtos;
    }

    public async Task AssignUserToTaskAsync(Guid taskId, Guid userId)
    {
        var existing = await _userTaskRepository.GetByIdAsync(userId, taskId);
        if (existing != null)
            return;

        var userTask = new UserTask
        {
            UserId = userId,
            TaskId = taskId,
            CreatedAt = DateTime.UtcNow
        };

        await _userTaskRepository.AddAsync(userTask);

        await _cacheService.RemoveAsync(CacheKeys.TasksByUser(userId));
    }

}
