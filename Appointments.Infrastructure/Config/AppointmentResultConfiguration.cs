using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Constants;

namespace Appointments.Infrastructure.Config;

public class AppointmentResultConfiguration : IEntityTypeConfiguration<AppointmentResult>
{
    public void Configure(EntityTypeBuilder<AppointmentResult> builder)
    {
        builder.ToTable("AppointmentResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Complaints)
            .IsRequired()
            .HasMaxLength(AppointmentConstants.MaxComplaintsLength);

        builder.Property(r => r.Conclusion)
            .IsRequired()
            .HasMaxLength(AppointmentConstants.MaxConclusionLength);

        builder.Property(r => r.Recommendations)
            .IsRequired()
            .HasMaxLength(AppointmentConstants.MaxRecommendationsLength);

        builder.HasIndex(r => r.AppointmentId)
            .IsUnique();
    }
}