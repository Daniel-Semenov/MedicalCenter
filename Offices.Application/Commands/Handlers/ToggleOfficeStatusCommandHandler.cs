using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class ToggleOfficeStatusCommandHandler : IRequestHandler<ToggleOfficeStatusCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxUnitOfWork _outboxUnitOfWork;

    public ToggleOfficeStatusCommandHandler(
        IOfficeRepository officeRepository,
        IPublishEndpoint publishEndpoint,
        IOutboxUnitOfWork outboxUnitOfWork)
    {
        _officeRepository = officeRepository;
        _publishEndpoint = publishEndpoint;
        _outboxUnitOfWork = outboxUnitOfWork;
    }

    public async Task Handle(ToggleOfficeStatusCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (office is null)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }

        office.ToggleStatus();

        await _officeRepository.UpdateAsync(office, cancellationToken);

        var integrationEvent = new OfficeStatusChanged
        {
            Id = office.Id,
            IsActive = office.IsActive,
            ChangedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        await _outboxUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}