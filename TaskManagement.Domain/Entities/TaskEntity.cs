using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public class TaskEntity : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public Guid CreatedBy { get; set; }
    public ICollection<UserTask> UserTasks { get; set; } = null!;
    public ICollection<TaskHistory> History { get; set; } = null!;
    public ICollection<TaskComment>? Comments { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}
