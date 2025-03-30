using TaskManagement.Common.Helpers;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Domain.Entities;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Services;

public class QueryTaskService : IQueryTaskService
{
    private readonly IQueryTaskRepository _queryTaskRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;

    public QueryTaskService(
        IQueryTaskRepository queryTaskRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository)
    {
        _queryTaskRepository = queryTaskRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskEntity>?> GetUserTasksAsync(Guid userId, TaskStatus? status, int page, int pageSize)
    {
        var userExists = await _userRepository.ExistAsync(userId);

        if (!userExists)
            throw new KeyNotFoundException("User not found");

        var tasks = await _queryTaskRepository.GetUserTasksAsync(userId, status, page, pageSize);
        if (tasks.IsNullOrEmpty())
            return null;

        return tasks;
    }

    public async Task<IEnumerable<TaskTimelineDto>?> GetTaskFullHistoryAsync(Guid taskId)
    {
        var taskExists = await _taskRepository.ExistAsync(taskId);

        if (!taskExists)
            throw new KeyNotFoundException("Task not found");

        var taskTimelines = await _queryTaskRepository.GetTaskFullHistoryAsync(taskId);
        if (taskTimelines.IsNullOrEmpty())
            return null;

        return taskTimelines.Select(timeline => new TaskTimelineDto
        {
            TaskId = timeline.TaskId,
            Status = timeline.Status,
            CommentText = timeline.CommentText,
            DateModified = timeline.DateModified
        });
    }

    public async Task<IEnumerable<User>?> GetTaskContributorsAsync(Guid taskId)
    {
        var taskExists = await _taskRepository.ExistAsync(taskId);

        if (!taskExists)
            throw new KeyNotFoundException("Task not found");

        var contributors = await _queryTaskRepository.GetTaskContributorsAsync(taskId);

        if (contributors.IsNullOrEmpty())
            return null;

        return contributors;
    }
}
