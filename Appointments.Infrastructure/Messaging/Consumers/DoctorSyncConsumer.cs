using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Doctors;
using Microsoft.Extensions.Logging;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class DoctorSyncConsumer :
    IConsumer<DoctorCreated>,
    IConsumer<DoctorUpdated>,
    IConsumer<DoctorDeleted>
{
    private readonly ILogger<DoctorSyncConsumer> _logger;
    private readonly ISnapshotRepository<DoctorSnapshot> _doctorSnapshotRepository;

    public DoctorSyncConsumer(
        ILogger<DoctorSyncConsumer> logger,
        ISnapshotRepository<DoctorSnapshot> doctorSnapshotRepository)
    {
        _logger = logger;
        _doctorSnapshotRepository = doctorSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<DoctorCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received DoctorCreated event: DoctorId={DoctorId}",
            message.Id);

        var snapshot = new DoctorSnapshot
        {
            Id = message.Id,
            FirstName = message.FirstName,
            LastName = message.LastName,
            MiddleName = message.MiddleName,
            SpecializationId = message.SpecializationId,
            OfficeId = message.OfficeId,
            IsActive = true
        };

        await _doctorSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "DoctorSnapshot created for DoctorId: {DoctorId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<DoctorUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received DoctorUpdated event: DoctorId={DoctorId}",
            message.Id);

        var existingResult = await _doctorSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        DoctorSnapshot snapshot;

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "DoctorSnapshot not found for DoctorId: {DoctorId} on update, creating new snapshot",
                message.Id);

            snapshot = new DoctorSnapshot
            {
                Id = message.Id,
                IsActive = true
            };
        }
        else
        {
            snapshot = existingResult.Value;
        }

        snapshot.FirstName = message.FirstName;
        snapshot.LastName = message.LastName;
        snapshot.MiddleName = message.MiddleName;
        snapshot.SpecializationId = message.SpecializationId;
        snapshot.OfficeId = message.OfficeId;

        await _doctorSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "DoctorSnapshot upserted for DoctorId: {DoctorId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<DoctorDeleted> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received DoctorDeleted event: DoctorId={DoctorId}",
            message.Id);

        var existingResult = await _doctorSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "DoctorSnapshot not found for DoctorId: {DoctorId} on delete",
                message.Id);

            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _doctorSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "DoctorSnapshot soft-deactivated for DoctorId: {DoctorId}",
            message.Id);
    }
}