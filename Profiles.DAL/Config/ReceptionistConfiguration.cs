using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.DAL.Entities;

namespace Profiles.DAL.Config;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.AccountId)
            .IsUnique();

        builder.HasIndex(r => r.OfficeId);
    }
}