using Microsoft.EntityFrameworkCore;

namespace Notifications.Infrastructure.Db;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(
        DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.Entities.Notification> Notifications => Set<Domain.Entities.Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NotificationsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}