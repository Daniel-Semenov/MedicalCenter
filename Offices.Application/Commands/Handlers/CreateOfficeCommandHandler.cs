using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;
using Offices.Domain.Entities;

namespace Offices.Application.Commands.Handlers;

public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublisher _publisher;

    public CreateOfficeCommandHandler(IOfficeRepository officeRepository, IPublisher publisher)
    {
        _officeRepository = officeRepository;
        _publisher = publisher;
    }

    public async Task<OfficeResponse> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = new Office(
            request.City,
            request.Street,
            request.HouseNumber,
            request.OfficeNumber,
            request.PhoneNumber);

        await _officeRepository.AddAsync(office, cancellationToken);

        await _publisher.Publish(new OfficeCreatedNotification(
            office.Id,
            office.City,
            office.Street,
            office.HouseNumber,
            office.OfficeNumber,
            office.PhoneNumber,
            office.IsActive,
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