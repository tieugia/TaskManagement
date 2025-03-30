using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.DTOs;

public class TaskTimelineDto
{
    public Guid TaskId { get; set; }
    public DateTime DateModified { get; set; }
    public TaskStatus? Status { get; set; }
    public string? CommentText { get; set; }
    public string? NotificationMessage { get; set; }
}
