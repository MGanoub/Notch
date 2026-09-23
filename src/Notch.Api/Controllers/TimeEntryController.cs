using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch.Api.Data;
using Notch.Api.Models;
using Notch.Shared.Dto;

namespace Notch.Api.Controllers;

[ApiController]
[Route("api/timeentries")]
[Authorize]
public class TimeEntryController : ControllerBase
{
    private readonly NotchDbContext _db;

    public TimeEntryController(NotchDbContext db)
    {
        _db = db;
    }

    [HttpPost("start")]
    public async Task<ActionResult<TimeEntryDto>> StartTask(StartTimerRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var task = await _db.TaskItems
            .AnyAsync(t => t.Id == request.TaskItemId && t.UserId == userId);
        if (!task)
        {
            return NotFound("Task not found");
        }
        
        // auto-stop whatever's currently running
        var currentTimeEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(e => e.EndedAt == null && e.UserId == userId);
        if (currentTimeEntry is not null)
        {
            currentTimeEntry.EndedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        var timeEntry = new TimeEntry
        {
            TaskItemId = request.TaskItemId,
            UserId = userId!,
        };
        await _db.TimeEntries.AddAsync(timeEntry);
        await _db.SaveChangesAsync();
        var dto = new TimeEntryDto(timeEntry.Id, timeEntry.TaskItemId, timeEntry.StartedAt, timeEntry.EndedAt);
        return Ok(dto);
    }

    [HttpPost("stop")]
    public async Task<ActionResult<TimeEntryDto>> StopTask()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentTimeEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(t => t.EndedAt == null && t.UserId == userId);
        if (currentTimeEntry is null)
        {
            return NoContent();
        }

        currentTimeEntry.EndedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        var currentTimeDto = new TimeEntryDto(currentTimeEntry.Id,  currentTimeEntry.TaskItemId,  currentTimeEntry.StartedAt, currentTimeEntry.EndedAt);
        return Ok(currentTimeDto);
    }
    
    [HttpGet("current")]
    public async Task<ActionResult<TimeEntryDto?>> GetCurrent()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var currentTimeEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(t => t.EndedAt == null && t.UserId == userId);

        if (currentTimeEntry is null)
        {
            return Ok();
        }
        var currentTimeDto = new TimeEntryDto(currentTimeEntry.Id,  currentTimeEntry.TaskItemId,  currentTimeEntry.StartedAt, currentTimeEntry.EndedAt);
        return Ok(currentTimeDto);
    }
}