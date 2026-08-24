using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Offices;
using Microsoft.Extensions.Logging;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class OfficeSyncConsumer :
    IConsumer<OfficeCreated>,
    IConsumer<OfficeUpdated>,
    IConsumer<OfficeStatusChanged>,
    IConsumer<OfficeDeleted>
{
    private readonly ILogger<OfficeSyncConsumer> _logger;
    private readonly ISnapshotRepository<OfficeSnapshot> _officeSnapshotRepository;

    public OfficeSyncConsumer(
        ILogger<OfficeSyncConsumer> logger,
        ISnapshotRepository<OfficeSnapshot> officeSnapshotRepository)
    {
        _logger = logger;
        _officeSnapshotRepository = officeSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<OfficeCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received OfficeCreated event: OfficeId={OfficeId}", message.Id);

        var snapshot = new OfficeSnapshot
        {
            Id = message.Id,
            Address = FormatAddress(message.City, message.Street, message.HouseNumber, message.OfficeNumber),
            IsActive = message.IsActive
        };

        await _officeSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("OfficeSnapshot created for OfficeId: {OfficeId}", message.Id);
    }

    public async Task Consume(ConsumeContext<OfficeUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received OfficeUpdated event: OfficeId={OfficeId}", message.Id);

        var existingResult = await _officeSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

        OfficeSnapshot snapshot;

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning(
                "OfficeSnapshot not found for OfficeId: {OfficeId} on update, creating new snapshot",
                message.Id);

            snapshot = new OfficeSnapshot
            {
                Id = message.Id,
                IsActive = true
            };
        }
        else
        {
            snapshot = existingResult.Value;
        }

        snapshot.Address = FormatAddress(message.City, message.Street, message.HouseNumber, message.OfficeNumber);

        await _officeSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("OfficeSnapshot upserted for OfficeId: {OfficeId}", message.Id);
    }

    public async Task Consume(ConsumeContext<OfficeStatusChanged> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received OfficeStatusChanged event: OfficeId={OfficeId}", message.Id);

        var existingResult = await _officeSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning("OfficeSnapshot not found for OfficeId: {OfficeId} on status change", message.Id);
            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = message.IsActive;

        await _officeSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation(
            "OfficeSnapshot status changed for OfficeId: {OfficeId}, IsActive: {IsActive}",
            message.Id, message.IsActive);
    }

    public async Task Consume(ConsumeContext<OfficeDeleted> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received OfficeDeleted event: OfficeId={OfficeId}", message.Id);

        var existingResult = await _officeSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

        if (!existingResult.IsSuccess || existingResult.Value is null)
        {
            _logger.LogWarning("OfficeSnapshot not found for OfficeId: {OfficeId} on delete", message.Id);
            return;
        }

        var snapshot = existingResult.Value;
        snapshot.IsActive = false;

        await _officeSnapshotRepository.UpsertAsync(snapshot, context.CancellationToken);

        _logger.LogInformation("OfficeSnapshot soft-deactivated for OfficeId: {OfficeId}", message.Id);
    }

    private static string FormatAddress(string city, string street, int houseNumber, int officeNumber) =>
        $"{city}, {street} {houseNumber}/{officeNumber}";
}