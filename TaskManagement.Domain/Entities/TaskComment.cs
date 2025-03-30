namespace TaskManagement.Domain.Entities;

public class TaskComment : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid HistoryId { get; set; }
    public TaskEntity Task { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string CommentText { get; set; } = null!;
}
