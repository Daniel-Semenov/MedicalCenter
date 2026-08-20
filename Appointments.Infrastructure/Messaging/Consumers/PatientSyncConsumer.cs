using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Infrastructure.Messaging.Contracts.Events.Patients;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class PatientSyncConsumer :
    IConsumer<PatientCreated>,
    IConsumer<PatientUpdated>,
    IConsumer<PatientDeleted>
{
    private readonly ILogger<PatientSyncConsumer> _logger;
    private readonly ISnapshotRepository<PatientSnapshot> _patientSnapshotRepository;

    public PatientSyncConsumer(
        ILogger<PatientSyncConsumer> logger,
        ISnapshotRepository<PatientSnapshot> patientSnapshotRepository)
    {
        _logger = logger;
        _patientSnapshotRepository = patientSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<PatientCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received PatientCreated event: PatientId={PatientId}", message.Id);

        var snapshot = new PatientSnapshot
        {
            Id = message.Id,
            FirstName = message.FirstName,
            LastName = message.LastName,
            MiddleName = message.MiddleName,        
            DateOfBirth = message.DateOfBirth,
            IsActive = true
        };

        await _patientSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("PatientSnapshot created for PatientId: {PatientId}", message.Id);
    }

    public async Task Consume(ConsumeContext<PatientUpdated> context)
    {
        var message = context.Message;

        var existingResult = await _patientSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            return;
        }

        var snapshot = existingResult.Value;

        snapshot.FirstName = message.FirstName;
        snapshot.LastName = message.LastName;
        snapshot.MiddleName = message.MiddleName;
        snapshot.DateOfBirth = message.DateOfBirth;
        snapshot.IsActive = true;

        await _patientSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("PatientSnapshot updated for PatientId: {PatientId}", message.Id);
    }
    
    
    public async Task Consume(ConsumeContext<PatientDeleted> context)
    {
        var message = context.Message;

        var existingResult = await _patientSnapshotRepository.GetByIdAsync(
            message.Id, 
            context.CancellationToken);

        if (!existingResult.IsSuccess)
        {
            _logger.LogError(
                "Failed to get PatientSnapshot for PatientId: {PatientId}. Error: {Error}", 
                message.Id, 
                existingResult.Error);
            return;
        }

        if (existingResult.Value is null)
        {
            _logger.LogWarning(
                "PatientSnapshot not found for PatientId: {PatientId}", 
                message.Id);
            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;  

        await _patientSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation(
            "PatientSnapshot marked as not active for PatientId: {PatientId}", 
            message.Id);
    }
}