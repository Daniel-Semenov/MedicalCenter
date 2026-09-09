using Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.Db;

public class DocumentsDbContext : DbContext
{
    public DocumentsDbContext(
        DbContextOptions<DocumentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DocumentsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}