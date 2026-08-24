using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Services;
using Microsoft.Extensions.Logging;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class ServiceSyncConsumer :
    IConsumer<ServiceCreated>,
    IConsumer<ServiceUpdated>,
    IConsumer<ServiceDeleted>
{
    private readonly ILogger<ServiceSyncConsumer> _logger;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;

    public ServiceSyncConsumer(
        ILogger<ServiceSyncConsumer> logger,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository)
    {
        _logger = logger;
        _serviceSnapshotRepository = serviceSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<ServiceCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received ServiceCreated event: ServiceId={ServiceId}",
            message.Id);

        var snapshot = new ServiceSnapshot
        {
            Id = message.Id,
            Name = message.Name,
            SpecializationId = message.SpecializationId,
            IsActive = message.Status == ServiceStatusSnapshot.Active
        };

        await _serviceSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "ServiceSnapshot created for ServiceId: {ServiceId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<ServiceUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received ServiceUpdated event: ServiceId={ServiceId}",
            message.Id);

        var existingResult = await _serviceSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        ServiceSnapshot snapshot;

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "ServiceSnapshot not found for ServiceId: {ServiceId} on update, creating new snapshot",
                message.Id);

            snapshot = new ServiceSnapshot
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
        snapshot.SpecializationId = message.SpecializationId;
        snapshot.IsActive = message.Status == ServiceStatusSnapshot.Active;

        await _serviceSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "ServiceSnapshot upserted for ServiceId: {ServiceId}",
            message.Id);
    }

    public async Task Consume(ConsumeContext<ServiceDeleted> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received ServiceDeleted event: ServiceId={ServiceId}",
            message.Id);

        var existingResult = await _serviceSnapshotRepository.GetByIdAsync(
            message.Id,
            context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "ServiceSnapshot not found for ServiceId: {ServiceId} on delete",
                message.Id);

            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _serviceSnapshotRepository.UpsertAsync(
            snapshot,
            context.CancellationToken);

        _logger.LogInformation(
            "ServiceSnapshot soft-deactivated for ServiceId: {ServiceId}",
            message.Id);
    }
}