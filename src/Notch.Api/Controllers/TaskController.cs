using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch.Api.Data;
using Notch.Api.Models;
using Notch.Shared.Dto;

namespace Notch.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly NotchDbContext _db;
    
    public TaskController(NotchDbContext db)
    {
        _db = db;
    }


    [HttpGet]
    public async Task<ActionResult<List<TaskItemDto>>> GetAll()
    {
        var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var taskList = await _db.TaskItems
            .Where(t => t.UserId == userid)
            .Select(t => new TaskItemDto(t.Id, t.Title, t.ParentTaskId, t.Status, t.CreatedAt))
            .ToListAsync();

        return Ok(taskList);
    }

    [HttpPost("")]
    public async Task<ActionResult> CreateTask(CreateTaskRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (request.ParentTaskId is not null)
        {
            var parentExists = await _db.TaskItems
                .AnyAsync(t => t.Id == request.ParentTaskId && t.UserId == userId);
            if (!parentExists)
            {
                return BadRequest("Parent task not found");
            }
        }

        var task = new TaskItem
        {
            Title = request.Title,
            ParentTaskId = request.ParentTaskId,
            UserId = userId!
        };
        _db.TaskItems.AddAsync(task);
        await _db.SaveChangesAsync();
        var dto = new TaskItemDto(task.Id, task.Title, task.ParentTaskId, task.Status, task.CreatedAt);
        return Created($"api/tasks/{task.Id}", dto);
    }
}