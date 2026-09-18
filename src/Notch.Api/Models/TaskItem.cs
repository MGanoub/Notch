using System.ComponentModel.DataAnnotations;
using Notch.Shared.Enums;
namespace Notch.Api.Models;

public class TaskItem
{
    [Required] public Guid Id { get; set; } = Guid.NewGuid();
       
    [Required] 
    public String Title { get; set; } = string.Empty;
    
    public Guid? ParentTaskId { get; set; } 
    public TaskItem? ParentTask { get; set; }
    
    public ICollection<TaskItem> Subtasks { get; set; } = new List<TaskItem>();

    public NotchStatus Status { get; set; } = NotchStatus.Todo;

    [Required] 
    public string UserId { get; set; } = string.Empty;
    
    public AppUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<TimeEntry> TimeEntries  { get; set; } = new List<TimeEntry>();

}