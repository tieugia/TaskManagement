using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskCommentRepository : GenericRepository<TaskComment>, ITaskCommentRepository
{
    public TaskCommentRepository(TaskManagementContext context) : base(context) { }
}
