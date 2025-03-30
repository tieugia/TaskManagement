using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Common.Attributes;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Presentation.Controllers;

/// <summary>
/// Controller for TaskInsights
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class TaskInsightsController : ControllerBase
{
    private readonly IQueryTaskService _taskQueryService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="taskQueryService"></param>
    public TaskInsightsController(IQueryTaskService taskQueryService)
    {
        _taskQueryService = taskQueryService;
    }

    /// <summary>
    /// Get tasks by user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="status"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("user/{userId}/tasks")]
    public async Task<IActionResult> GetUserTasks([NotEmptyGuid] Guid userId, [FromQuery] TaskStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _taskQueryService.GetUserTasksAsync(userId, status, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get task history
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    [HttpGet("{taskId}/history")]
    public async Task<IActionResult> GetTaskHistory([NotEmptyGuid] Guid taskId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _taskQueryService.GetTaskFullHistoryAsync(taskId);
        return Ok(result);
    }

    /// <summary>
    /// Get task contributors
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    [HttpGet("{taskId}/contributors")]
    public async Task<IActionResult> GetContributors([NotEmptyGuid] Guid taskId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _taskQueryService.GetTaskContributorsAsync(taskId);
        return Ok(result);
    }
}
