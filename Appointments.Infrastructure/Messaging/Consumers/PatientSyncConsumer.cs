using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Patients;
using Microsoft.Extensions.Logging;

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

        _logger.LogInformation(
            "Received PatientCreated event: PatientId={PatientId}",
            message.Id);

        var snapshot = new PatientSnapshot
        {
            Id = message.Id,
            FirstName = message.FirstName,
            LastName = message.LastName,
            MiddleName = message.MiddleName,
            DateOfBirth = message.DateOfBirth,
            IsActive = true
        };

        await _patientSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "PatientSnapshot created for PatientId: {PatientId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<PatientUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received PatientUpdated event: PatientId={PatientId}",
            message.Id);

        var existingResult = await _patientSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        PatientSnapshot snapshot;

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "PatientSnapshot not found for PatientId: {PatientId} on update, creating new snapshot",
                message.Id);

            snapshot = new PatientSnapshot
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
        snapshot.DateOfBirth = message.DateOfBirth;

        await _patientSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "PatientSnapshot upserted for PatientId: {PatientId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<PatientDeleted> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received PatientDeleted event: PatientId={PatientId}",
            message.Id);

        var existingResult = await _patientSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "PatientSnapshot not found for PatientId: {PatientId} on delete",
                message.Id);

            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _patientSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "PatientSnapshot soft-deactivated for PatientId: {PatientId}",
            message.Id);
    }
}