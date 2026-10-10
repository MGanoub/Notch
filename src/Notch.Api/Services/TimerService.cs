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

        var running = await _db.TimeEntries.Where(e => e.EndedAt == null && e.UserId == userId)
            .OrderByDescending(e => e.StartedAt)
            .ToListAsync();
        if (running.Count == 1 && running[0].TaskItemId == taskItemId)
        {
            return running[0];
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;
        foreach (var entry in running) entry.EndedAt = now;

        var stoppedIds = running.Select(e => e.TaskItemId).Distinct().ToList();
        var stoppedTasks =
            await _db.TaskItems.Where(t => stoppedIds.Contains(t.Id) && t.Status == NotchStatus.InProgress)
                .ToListAsync();
        foreach (var t in stoppedTasks) t.Status = NotchStatus.Todo;
        
        var stoppedSubtasks = await _db.TaskItems
            .Where(t => t.ParentTaskId != null
                        && stoppedIds.Contains(t.ParentTaskId.Value)
                        && t.Status == NotchStatus.InProgress)
            .ToListAsync();
        await ResetStoppedTasks(stoppedIds);

        await _db.SaveChangesAsync();

        task.Status = NotchStatus.InProgress;
        var newEntry = new TimeEntry { TaskItemId = taskItemId, UserId = userId };
        _db.TimeEntries.Add(newEntry);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return newEntry;
    }
    
    public async Task<TimeEntry?> StopAsync(string userId)
    {
        var running = await _db.TimeEntries
            .Where(e => e.EndedAt == null && e.UserId == userId)
            .OrderByDescending(e => e.StartedAt)
            .ToListAsync();
        if (running.Count == 0) return null;

        var now = DateTime.UtcNow;
        foreach (var entry in running) entry.EndedAt = now;

        var ids = running.Select(e => e.TaskItemId).Distinct().ToList();
        await ResetStoppedTasks(ids);

        await _db.SaveChangesAsync();
        return running[0];
    }
    
    private async Task ResetStoppedTasks(List<Guid> taskIds)
    {
        var toReset = await _db.TaskItems
            .Where(t => t.Status == NotchStatus.InProgress
                        && (taskIds.Contains(t.Id)
                            || (t.ParentTaskId != null && taskIds.Contains(t.ParentTaskId.Value))))
            .ToListAsync();

        foreach (var t in toReset) t.Status = NotchStatus.Todo;
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