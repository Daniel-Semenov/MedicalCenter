using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class DoctorSyncConsumer :
    IConsumer<DoctorCreated>,
    IConsumer<DoctorUpdated>
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

        _logger.LogInformation("Received DoctorCreated event: DoctorId={DoctorId}", message.Id);

        var snapshot = new DoctorSnapshot
        {
            Id = message.Id,
            SpecializationId = message.SpecializationId,
            OfficeId = message.OfficeId,
            IsAtWork = true
        };

        await _doctorSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("DoctorSnapshot created for DoctorId: {DoctorId}", message.Id);
    }

    public async Task Consume(ConsumeContext<DoctorUpdated> context)
    {
        var message = context.Message;

        var existingResult = await _doctorSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            return;
        }

        var snapshot = existingResult.Value;

        snapshot.SpecializationId = message.SpecializationId;
        snapshot.OfficeId = message.OfficeId;
        snapshot.IsAtWork = true;

        await _doctorSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("DoctorSnapshot updated for DoctorId: {DoctorId}", message.Id);
    }
}