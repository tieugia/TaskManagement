using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Infrastructure.Repositories.Query;

public class QueryTaskRepository : IQueryTaskRepository
{
    private readonly TaskManagementContext _context;

    public QueryTaskRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetUserTasksAsync(Guid userId, TaskStatus? status, int page, int pageSize)
    {
        return await _context.Tasks
            .FromSqlInterpolated($@"EXEC sp_GetUserTasks @UserId = {userId}, @Status = {status}, @Page = {page}, @PageSize = {pageSize}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskTimeline>> GetTaskFullHistoryAsync(Guid taskId)
    {
        return await _context.Set<TaskTimeline>()
            .FromSqlInterpolated($@"EXEC sp_GetTaskFullHistory @TaskId = {taskId}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetTaskContributorsAsync(Guid taskId)
    {
        return await _context.Users
            .FromSqlInterpolated($@"EXEC sp_GetTaskContributors @TaskId = {taskId}")
            .AsNoTracking()
            .ToListAsync();
    }
}
