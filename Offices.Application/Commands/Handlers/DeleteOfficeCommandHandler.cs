using MediatR;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublisher _publisher;

    public DeleteOfficeCommandHandler(IOfficeRepository officeRepository, IPublisher publisher)
    {
        _officeRepository = officeRepository;
        _publisher = publisher;
    }

    public async Task Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _officeRepository.ExistsAsync(request.Id, cancellationToken);

        if (!exists)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }

        await _officeRepository.DeleteAsync(request.Id, cancellationToken);

        await _publisher.Publish(new OfficeDeletedNotification(
            request.Id,
            DateTime.UtcNow
        ), cancellationToken);
    }
}