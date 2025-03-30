namespace TaskManagement.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
}
