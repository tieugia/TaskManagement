using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public class TaskHistory : BaseEntity
{
    public Guid TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public Guid UpdatedByUserId { get; set; }
}
