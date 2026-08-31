using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxUnitOfWork _outboxUnitOfWork;

    public DeleteOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IPublishEndpoint publishEndpoint,
        IOutboxUnitOfWork outboxUnitOfWork)
    {
        _officeRepository = officeRepository;
        _publishEndpoint = publishEndpoint;
        _outboxUnitOfWork = outboxUnitOfWork;
    }

    public async Task Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _officeRepository.ExistsAsync(request.Id, cancellationToken);

        if (!exists)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }

        await _officeRepository.DeleteAsync(request.Id, cancellationToken);

        var integrationEvent = new OfficeDeleted
        {
            Id = request.Id,
            DeletedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        await _outboxUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}