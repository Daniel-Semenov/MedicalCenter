using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

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

        _logger.LogInformation("Received DoctorCreated event: DoctorId={DoctorId}", message.Id);

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

        snapshot.Id = message.Id;
        snapshot.FirstName = message.FirstName;
        snapshot.LastName = message.LastName;
        snapshot.MiddleName = message.MiddleName;
        snapshot.SpecializationId = message.SpecializationId;
        snapshot.OfficeId = message.OfficeId;
        snapshot.IsActive = true;

        await _doctorSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("DoctorSnapshot updated for DoctorId: {DoctorId}", message.Id);
    }
    
    public async Task Consume(ConsumeContext<DoctorDeleted> context)
    {
        var message = context.Message;

        var existingResult = await _doctorSnapshotRepository.GetByIdAsync(
            message.Id, 
            context.CancellationToken);

        if (!existingResult.IsSuccess)
        {
            _logger.LogError(
                "Failed to get DoctorSnapshot for DoctorId: {DoctorId}. Error: {Error}", 
                message.Id, 
                existingResult.Error);
            return;
        }

        if (existingResult.Value is null)
        {
            _logger.LogWarning(
                "DoctorSnapshot not found for DoctorId: {DoctorId}", 
                message.Id);
            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;  

        await _doctorSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation(
            "DoctorSnapshot marked as not active for DoctorId: {DoctorId}", 
            message.Id);
    }
}