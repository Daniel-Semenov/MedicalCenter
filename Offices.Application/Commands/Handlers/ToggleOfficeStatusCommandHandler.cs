using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Caching;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class ToggleOfficeStatusCommandHandler : IRequestHandler<ToggleOfficeStatusCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxUnitOfWork _outboxUnitOfWork;
    private readonly ICacheService _cache;

    public ToggleOfficeStatusCommandHandler(
        IOfficeRepository officeRepository,
        IPublishEndpoint publishEndpoint,
        IOutboxUnitOfWork outboxUnitOfWork, 
        ICacheService cache)
    {
        _officeRepository = officeRepository;
        _publishEndpoint = publishEndpoint;
        _outboxUnitOfWork = outboxUnitOfWork;
        _cache = cache;
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
        
        await _cache.BumpVersionAsync(OfficeCache.Scope, CancellationToken.None);
    }
}