using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublisher _publisher;

    public UpdateOfficeCommandHandler(IOfficeRepository officeRepository, IPublisher publisher)
    {
        _officeRepository = officeRepository;
        _publisher = publisher;
    }

    public async Task<OfficeResponse> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (office is null)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }

        office.UpdateDetails(
            request.City,
            request.Street,
            request.HouseNumber,
            request.OfficeNumber,
            request.PhoneNumber);

        await _officeRepository.UpdateAsync(office, cancellationToken);

        await _publisher.Publish(new OfficeUpdatedNotification(
            office.Id,
            office.City,
            office.Street,
            office.HouseNumber,
            office.OfficeNumber,
            office.PhoneNumber,
            DateTime.UtcNow
        ), cancellationToken);

        return new OfficeResponse
        {
            Id = office.Id,
            FullAddress = $"{office.City}, {office.Street} {office.HouseNumber}/{office.OfficeNumber}",
            IsActive = office.IsActive,
            PhoneNumber = office.PhoneNumber,
            PhotoUrl = office.PhotoUrl
        };
    }
}