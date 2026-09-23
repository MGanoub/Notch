using Microsoft.EntityFrameworkCore;
using Notch.Api.Data;
using Notch.Api.Models;
using Notch.Shared.Enums;

namespace Notch.Api.Services;

public class TimerService
{
    private readonly NotchDbContext _db;
    public TimerService(NotchDbContext db)
    {
        _db = db;
    }

    public async Task<TimeEntry?> StartAsync(string userId, Guid taskItemId)
    {
        var task = await _db.TaskItems
            .FirstOrDefaultAsync(t=>t.Id == taskItemId && t.UserId == userId);
        if (task is null)
        {
            return null;
        }

        var runningEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(e => e.EndedAt == null && e.UserId == userId);
        if (runningEntry is not null)
        {
            runningEntry.EndedAt = DateTime.UtcNow;
        }

        task.Status = NotchStatus.InProgress;
        var newEntry = new TimeEntry
        {
            TaskItemId = taskItemId,
            UserId = userId,
        };
        _db.TimeEntries.Add(newEntry);
        await _db.SaveChangesAsync();
        return newEntry;
    }
    
    public async Task<TimeEntry?> StopAsync(string userId)
    {
        var runningEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(e => e.EndedAt == null && e.UserId == userId);
 
        if (runningEntry is null)
        {
            return null;
        }
 
        runningEntry.EndedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
 
        return runningEntry;
    }
    
    public async Task<TimeEntry?> StopIfRunningForTaskAsync(string userId, Guid taskItemId)
    {
        var runningEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(e => e.EndedAt == null && 
                                      e.UserId == userId && 
                                      e.TaskItemId == taskItemId);
 
        if (runningEntry is null)
        {
            return null;
        }
 
        runningEntry.EndedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
 
        return runningEntry;
    }
}