using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Caching;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Entities;

namespace Offices.Application.Commands.Handlers;

public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxUnitOfWork _outboxUnitOfWork;
    private readonly IOfficeResponseFactory _officeResponseFactory;
    private readonly ICacheService _cache;

    public CreateOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IPublishEndpoint publishEndpoint,
        IOutboxUnitOfWork outboxUnitOfWork, 
        IOfficeResponseFactory officeResponseFactory, 
        ICacheService cache)
    {
        _officeRepository = officeRepository;
        _publishEndpoint = publishEndpoint;
        _outboxUnitOfWork = outboxUnitOfWork;
        _officeResponseFactory = officeResponseFactory;
        _cache = cache;
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
        
        await _cache.BumpVersionAsync(OfficeCache.Scope, CancellationToken.None);

        return await _officeResponseFactory.CreateAsync(office, cancellationToken);
    }
}