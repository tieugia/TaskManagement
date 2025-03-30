using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Common.Attributes;

namespace TaskManagement.Presentation.Controllers;

/// <summary>
/// Task controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="taskService"></param>
    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Retrieves a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([NotEmptyGuid] Guid id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var task = await _taskService.GetTaskByIdAsync(id);

        return Ok(task);
    }

    /// <summary>
    /// Gets tasks by user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of tasks</returns>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser([NotEmptyGuid] Guid userId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var tasks = await _taskService.GetTasksByUserAsync(userId);
        return Ok(tasks);
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="task">Task details</param>
    /// <returns>Created task</returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto task)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _taskService.CreateTaskAsync(task);

        if (created is null) return BadRequest("Invalid task format");

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="task">Updated task details</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([NotEmptyGuid] Guid id, UpdateTaskDto task)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (id != task.Id) return BadRequest();

        await _taskService.UpdateTaskAsync(task);
        return NoContent();
    }

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([NotEmptyGuid] Guid id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}
