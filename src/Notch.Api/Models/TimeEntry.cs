using System.ComponentModel.DataAnnotations;

namespace Notch.Api.Models;

public class TimeEntry
{
    public Guid Id  { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid TaskItemId { get; set; } = Guid.Empty;
    
    public TaskItem? TaskItem { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public AppUser? User { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndedAt { get; set; }
}