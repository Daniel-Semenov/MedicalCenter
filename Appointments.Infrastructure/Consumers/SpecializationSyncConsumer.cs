using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Appointments.Infrastructure.Messaging.Contracts.Enums;
using Appointments.Infrastructure.Messaging.Contracts.Specializations;

namespace Appointments.Infrastructure.Consumers;

public class SpecializationSyncConsumer :
    IConsumer<SpecializationCreated>,
    IConsumer<SpecializationUpdated>,
    IConsumer<SpecializationDeleted>
{
    private readonly ISnapshotRepository<SpecializationSnapshot> _specializationSnapshotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;

    public SpecializationSyncConsumer(
        ISnapshotRepository<SpecializationSnapshot> specializationSnapshotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository)
    {
        _specializationSnapshotRepository = specializationSnapshotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<SpecializationCreated> context)
    {
        var message = context.Message;

        var specializationSnapshot = new SpecializationSnapshot
        {
            Id = message.Id,
            Name = message.Name,
            IsActive = message.Status == SpecializationStatusSnapshot.Active
        };

        await _specializationSnapshotRepository.UpsertAsync(specializationSnapshot, context.CancellationToken);

        foreach (var serviceItem in message.Services)
        {
            var serviceSnapshot = new ServiceSnapshot
            {
                Id = serviceItem.Id,
                Name = serviceItem.Name,
                SpecializationId = message.Id,
                IsActive = serviceItem.Status == ServiceStatusSnapshot.Active
            };

            await _serviceSnapshotRepository.UpsertAsync(serviceSnapshot, context.CancellationToken);
        }
    }

    public async Task Consume(ConsumeContext<SpecializationUpdated> context)
    {
        var message = context.Message;

        var snapshot = new SpecializationSnapshot
        {
            Id = message.Id,
            Name = message.Name,
            IsActive = message.Status == SpecializationStatusSnapshot.Active
        };

        await _specializationSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<SpecializationDeleted> context)
    {
        var existingResult = await _specializationSnapshotRepository.GetByIdAsync(context.Message.Id, context.CancellationToken);

        if (!existingResult.IsSuccess)
        {
            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _specializationSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);
    }
}