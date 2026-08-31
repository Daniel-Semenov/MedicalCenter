using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Entities;

namespace Offices.Application.Commands.Handlers;

public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxUnitOfWork _outboxUnitOfWork;

    public CreateOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IPublishEndpoint publishEndpoint,
        IOutboxUnitOfWork outboxUnitOfWork)
    {
        _officeRepository = officeRepository;
        _publishEndpoint = publishEndpoint;
        _outboxUnitOfWork = outboxUnitOfWork;
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

        var integrationEvent = new OfficeCreated
        {
            Id = office.Id,
            City = office.City,
            Street = office.Street,
            HouseNumber = office.HouseNumber,
            OfficeNumber = office.OfficeNumber,
            PhoneNumber = office.PhoneNumber,
            IsActive = office.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        await _outboxUnitOfWork.SaveChangesAsync(cancellationToken);

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