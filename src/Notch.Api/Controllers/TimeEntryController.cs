using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch.Api.Data;
using Notch.Api.Models;
using Notch.Api.Services;
using Notch.Shared.Dto;

namespace Notch.Api.Controllers;

[ApiController]
[Route("api/timeentries")]
[Authorize]
public class TimeEntryController : ControllerBase
{
    private readonly NotchDbContext _db;
    private readonly TimerService _timerService;

    public TimeEntryController(NotchDbContext db, TimerService timerService)
    {
        _db = db;
        _timerService = timerService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<TimeEntryDto>> StartTask(StartTimerRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var entry = await _timerService.StartAsync(userId, request.TaskItemId);
        if (entry is null)
        {
            return NotFound("Task item not found");
        }
        var dto = new TimeEntryDto(entry.Id, entry.TaskItemId, entry.StartedAt, entry.EndedAt);
        return Ok(dto);
    }

    [HttpPost("stop")]
    public async Task<ActionResult<TimeEntryDto>> StopTask()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var entry = await _timerService.StopAsync(userId);
        if (entry is null)
        {
            return NoContent();
        }
        var currentTimeDto = new TimeEntryDto(entry.Id,  entry.TaskItemId,  entry.StartedAt, entry.EndedAt);
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