using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskHistoryRepository : GenericRepository<TaskHistory>, ITaskHistoryRepository
{
    public TaskHistoryRepository(TaskManagementContext context) : base(context) { }
}
