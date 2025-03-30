using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.DTOs;

public class TaskFilterDto
{
    public TaskStatus? Status { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
