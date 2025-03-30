using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class UserTaskRepository : GenericRepository<UserTask>, IUserTaskRepository
{
    public UserTaskRepository(TaskManagementContext context) : base(context) { }

    public Task<UserTask?> GetByIdAsync(Guid userId, Guid taskId)
        => Query().SingleOrDefaultAsync(ut => ut.UserId == userId && ut.TaskId == taskId);

    public async Task<IEnumerable<TaskEntity>> FilterTasksAsync(Guid userId, TaskFilterDto filter)
    {
        var query = Where(ut => ut.UserId == userId)
                   .Select(x => x.Task);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(t => t.Title.Contains(filter.Title));

        if (!string.IsNullOrWhiteSpace(filter.Description))
            query = query.Where(t => t.Description != null && t.Description.Contains(filter.Description));

        query = query.OrderByDescending(t => t.CreatedAt)
                     .Skip((filter.Page - 1) * filter.PageSize)
                     .Take(filter.PageSize);

        return await query.ToListAsync();
    }
}
