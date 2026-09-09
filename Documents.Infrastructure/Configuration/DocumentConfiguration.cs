using Documents.Domain.Entities;
using Documents.Shared.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documents.Infrastructure.Configuration;

public sealed class DocumentConfiguration
    : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents"); 
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.OwnerType)
            .IsRequired();

        builder.Property(x => x.OwnerId)
            .IsRequired();

        builder.Property(x => x.Bucket)
            .HasMaxLength(ValidationConstants.MaxBucketLength)
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasMaxLength(ValidationConstants.MaxObjectKeyLength)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(ValidationConstants.MaxContentTypeLength)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasMaxLength(ValidationConstants.MaxFileNameLength)
            .IsRequired();

        builder.Property(x => x.SizeBytes)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.OwnerType,
            x.OwnerId
        });

        builder.HasIndex(x => x.ObjectKey)
            .IsUnique();
    }
}