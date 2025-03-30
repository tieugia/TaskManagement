using Microsoft.EntityFrameworkCore;
using System;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;

public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entity.ClrType))
            {
                modelBuilder.Entity(entity.ClrType)
                    .Property("RowVersion")
                    .IsRowVersion();
            }
        }
    }
}
