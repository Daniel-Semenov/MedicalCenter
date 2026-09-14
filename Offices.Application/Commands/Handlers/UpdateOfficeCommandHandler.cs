using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxUnitOfWork _outboxUnitOfWork;
    private readonly IOfficeResponseFactory _officeResponseFactory;

    public UpdateOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IPublishEndpoint publishEndpoint,
        IOutboxUnitOfWork outboxUnitOfWork, 
        IOfficeResponseFactory officeResponseFactory)
    {
        _officeRepository = officeRepository;
        _publishEndpoint = publishEndpoint;
        _outboxUnitOfWork = outboxUnitOfWork;
        _officeResponseFactory = officeResponseFactory;
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

        var integrationEvent = new OfficeUpdated
        {
            Id = office.Id,
            City = office.City,
            Street = office.Street,
            HouseNumber = office.HouseNumber,
            OfficeNumber = office.OfficeNumber,
            PhoneNumber = office.PhoneNumber,
            UpdatedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        await _outboxUnitOfWork.SaveChangesAsync(cancellationToken);
        return await _officeResponseFactory.CreateAsync(office, cancellationToken);
    }
}