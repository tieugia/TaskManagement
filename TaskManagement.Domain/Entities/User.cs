using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
    public ICollection<UserTask>? UserTasks { get; set; }
    public ICollection<TaskComment>? Comments { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}
