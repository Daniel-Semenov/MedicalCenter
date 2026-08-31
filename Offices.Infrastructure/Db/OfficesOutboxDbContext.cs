using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Offices.Infrastructure.Db;

public class OfficesOutboxDbContext : DbContext
{
    public OfficesOutboxDbContext(DbContextOptions<OfficesOutboxDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}