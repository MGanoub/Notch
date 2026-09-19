
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notch.Api.Models;

namespace Notch.Api.Data;

public class NotchDbContext : IdentityDbContext<AppUser>
{
    public NotchDbContext(DbContextOptions<NotchDbContext> options)
    : base(options)
    {
    }
    
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TimeEntry>  TimeEntries { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<TaskItem>()
            .HasOne(t => t.ParentTask)
            .WithMany(t => t.Subtasks)
            .HasForeignKey(t=>t.ParentTaskId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TaskItem>()
            .HasIndex(t => new { t.UserId, t.Status });

        builder.Entity<TimeEntry>()
            .HasOne(t => t.TaskItem)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(e => e.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TimeEntry>()
            .HasIndex(t => new { t.UserId, t.StartedAt });
        builder.Entity<RefreshToken>()
            .HasIndex(t => t.TokenHash).IsUnique();
    }
}