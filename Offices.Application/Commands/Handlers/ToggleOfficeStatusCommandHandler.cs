using MediatR;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class ToggleOfficeStatusCommandHandler : IRequestHandler<ToggleOfficeStatusCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublisher _publisher;

    public ToggleOfficeStatusCommandHandler(IOfficeRepository officeRepository, IPublisher publisher)
    {
        _officeRepository = officeRepository;
        _publisher = publisher;
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

        await _publisher.Publish(new OfficeStatusChangedNotification(
            office.Id,
            office.IsActive,
            DateTime.UtcNow
        ), cancellationToken);
    }
}