using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Specializations;
using Microsoft.Extensions.Logging;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class SpecializationSyncConsumer :
    IConsumer<SpecializationCreated>,
    IConsumer<SpecializationUpdated>,
    IConsumer<SpecializationDeleted>
{
    private readonly ILogger<SpecializationSyncConsumer> _logger;
    private readonly ISnapshotRepository<SpecializationSnapshot> _specializationSnapshotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;

    public SpecializationSyncConsumer(
        ILogger<SpecializationSyncConsumer> logger,
        ISnapshotRepository<SpecializationSnapshot> specializationSnapshotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository)
    {
        _logger = logger;
        _specializationSnapshotRepository = specializationSnapshotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<SpecializationCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received SpecializationCreated event: SpecializationId={SpecializationId}",
            message.Id);

        var specializationSnapshot = new SpecializationSnapshot
        {
            Id = message.Id,
            Name = message.Name,
            IsActive = message.Status == SpecializationStatusSnapshot.Active
        };

        await _specializationSnapshotRepository.UpsertAsync(
            specializationSnapshot,
            context.CancellationToken);

        foreach (var serviceItem in message.Services)
        {
            var serviceSnapshot = new ServiceSnapshot
            {
                Id = serviceItem.Id,
                Name = serviceItem.Name,
                SpecializationId = message.Id,
                IsActive = serviceItem.Status == ServiceStatusSnapshot.Active
            };

            await _serviceSnapshotRepository.UpsertAsync(
                serviceSnapshot,
                context.CancellationToken);
        }

        _logger.LogInformation(
            "SpecializationSnapshot created for SpecializationId: {SpecializationId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<SpecializationUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received SpecializationUpdated event: SpecializationId={SpecializationId}",
            message.Id);

        var existingResult = await _specializationSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        SpecializationSnapshot snapshot;

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "SpecializationSnapshot not found for SpecializationId: {SpecializationId} on update, creating new snapshot",
                message.Id);

            snapshot = new SpecializationSnapshot
            {
                Id = message.Id,
                IsActive = true
            };
        }
        else
        {
            snapshot = existingResult.Value;
        }

        snapshot.Name = message.Name;
        snapshot.IsActive = message.Status == SpecializationStatusSnapshot.Active;

        await _specializationSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "SpecializationSnapshot upserted for SpecializationId: {SpecializationId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<SpecializationDeleted> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received SpecializationDeleted event: SpecializationId={SpecializationId}",
            message.Id);

        var existingResult = await _specializationSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "SpecializationSnapshot not found for SpecializationId: {SpecializationId} on delete",
                message.Id);

            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _specializationSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "SpecializationSnapshot soft-deactivated for SpecializationId: {SpecializationId}",
            message.Id);
    }
}