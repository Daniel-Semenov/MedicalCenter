using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Services;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class ServiceSyncConsumer :
    IConsumer<ServiceCreated>,
    IConsumer<ServiceUpdated>,
    IConsumer<ServiceDeleted> 
{
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;

    public ServiceSyncConsumer(ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository)
    {
        _serviceSnapshotRepository = serviceSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<ServiceCreated> context)
    {
        var message = context.Message;

        var snapshot = new ServiceSnapshot
        {
            Id = message.Id,
            Name = message.Name,
            SpecializationId = message.SpecializationId,
            IsActive = message.Status == ServiceStatusSnapshot.Active
        };

        await _serviceSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<ServiceUpdated> context)
    {
        var message = context.Message;

        var snapshot = new ServiceSnapshot
        {
            Id = message.Id,
            Name = message.Name,
            SpecializationId = message.SpecializationId,
            IsActive = message.Status == ServiceStatusSnapshot.Active
        };

        await _serviceSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<ServiceDeleted> context)
    {
        var existingResult = await _serviceSnapshotRepository.GetByIdAsync(context.Message.Id, context.CancellationToken);

        if (!existingResult.IsSuccess)
        {
            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _serviceSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);
    }
}