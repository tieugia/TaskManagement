namespace TaskManagement.Domain.Entities;

public class UserTask : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
}
