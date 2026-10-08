using Appointments.Application.Extensions;
using Appointments.Application.Interfaces;
using Appointments.Application.Options;
using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Infrastructure.Db;
using Appointments.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.PostgreSql;
using Xunit;

namespace Appointments.Tests.Integration;

/// <summary>
/// One real PostgreSQL container for the whole test run. The schema is created by the real EF migrations,
/// so the migrations themselves are exercised too. Every test uses its own freshly generated ids
/// (doctor, patient, ...), so tests do not need DB cleanup and do not affect each other.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public ServiceProvider Provider { get; private set; } = null!;
    public Mock<IDocumentsClient> Documents { get; } = new();
    public Mock<IPublishEndpoint> Publish { get; } = new();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        Documents
            .Setup(d => d.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Guid.NewGuid());

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplicationLayer();

        services.AddDbContext<AppointmentsDbContext>(o => o.UseNpgsql(_container.GetConnectionString()));

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentResultRepository, AppointmentResultRepository>();
        services.AddScoped<ISnapshotRepository<DoctorSnapshot>, SnapshotRepository<DoctorSnapshot>>();
        services.AddScoped<ISnapshotRepository<PatientSnapshot>, SnapshotRepository<PatientSnapshot>>();
        services.AddScoped<ISnapshotRepository<SpecializationSnapshot>, SnapshotRepository<SpecializationSnapshot>>();
        services.AddScoped<ISnapshotRepository<ServiceSnapshot>, SnapshotRepository<ServiceSnapshot>>();
        services.AddScoped<ISnapshotRepository<OfficeSnapshot>, SnapshotRepository<OfficeSnapshot>>();
        services.AddScoped<IDoctorScheduleSnapshotRepository, DoctorScheduleSnapshotRepository>();
        services.AddScoped<IDoctorSnapshotQuery, DoctorSnapshotQuery>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

        services.Configure<TimeSlotGenerationOptions>(o => o.WindowDays = 14);

        services.AddSingleton(Documents.Object);
        services.AddSingleton(Publish.Object);

        Provider = services.BuildServiceProvider();

        using var scope = Provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppointmentsDbContext>().Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Provider.DisposeAsync();
        await _container.DisposeAsync();
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = Provider.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
    }

    public async Task<T> QueryDbAsync<T>(Func<AppointmentsDbContext, Task<T>> query)
    {
        using var scope = Provider.CreateScope();
        return await query(scope.ServiceProvider.GetRequiredService<AppointmentsDbContext>());
    }

    public async Task ExecuteDbAsync(Func<AppointmentsDbContext, Task> action)
    {
        using var scope = Provider.CreateScope();
        await action(scope.ServiceProvider.GetRequiredService<AppointmentsDbContext>());
    }
}

[CollectionDefinition(Name)]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Postgres";
}

public sealed record Seed(
    Guid PatientId,
    string PatientEmail,
    Guid DoctorId,
    Guid ServiceId,
    Guid SpecializationId,
    Guid OfficeId);

public static class TestData
{
    public static DateTime FutureSlotStart(int daysAhead = 1, int hour = 9, int minute = 0) =>
        DateTime.UtcNow.Date.AddDays(daysAhead).AddHours(hour).AddMinutes(minute);

    public static async Task<Seed> SeedAsync(
        PostgresFixture fx,
        bool withService = true,
        bool doctorHasOffice = true,
        bool doctorActive = true)
    {
        var seed = new Seed(
            PatientId: Guid.NewGuid(),
            PatientEmail: $"patient-{Guid.NewGuid():N}@test.local",
            DoctorId: Guid.NewGuid(),
            ServiceId: Guid.NewGuid(),
            SpecializationId: Guid.NewGuid(),
            OfficeId: Guid.NewGuid());

        await fx.ExecuteDbAsync(async db =>
        {
            db.SpecializationSnapshots.Add(new SpecializationSnapshot
                { Id = seed.SpecializationId, Name = "Neurology", IsActive = true });

            if (withService)
            {
                db.ServiceSnapshots.Add(new ServiceSnapshot
                    { Id = seed.ServiceId, Name = "Consultation", SpecializationId = seed.SpecializationId, IsActive = true });
            }

            db.OfficeSnapshots.Add(new OfficeSnapshot
                { Id = seed.OfficeId, Address = "Minsk, Main 1/1", IsActive = true });

            db.DoctorSnapshots.Add(new DoctorSnapshot
            {
                Id = seed.DoctorId,
                FirstName = "Doctor",
                LastName = "Doctorov",
                SpecializationId = seed.SpecializationId,
                OfficeId = doctorHasOffice ? seed.OfficeId : null,
                IsActive = doctorActive
            });

            db.PatientSnapshots.Add(new PatientSnapshot
            {
                Id = seed.PatientId,
                Email = seed.PatientEmail,
                FirstName = "Danya",
                LastName = "Danyonov",
                IsActive = true
            });

            await db.SaveChangesAsync();
        });

        return seed;
    }

    public static async Task<Guid> AddSlotAsync(
        PostgresFixture fx,
        Seed seed,
        DateTime start,
        TimeSlotStatus status = TimeSlotStatus.Available)
    {
        var id = Guid.NewGuid();

        await fx.ExecuteDbAsync(async db =>
        {
            db.TimeSlots.Add(new TimeSlot
            {
                Id = id,
                DoctorId = seed.DoctorId,
                OfficeId = seed.OfficeId,
                StartTime = start,
                EndTime = start.AddMinutes(20),
                Status = status
            });

            await db.SaveChangesAsync();
        });

        return id;
    }
}
