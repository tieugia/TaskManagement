using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

[NotMapped]
public class TaskTimeline
{
    public Guid TaskId { get; set; }
    public DateTime DateModified { get; set; }
    public TaskStatus? Status { get; set; }
    public string? CommentText { get; set; }
    public string? NotificationMessage { get; set; }
}
