using MediatR;
using Offices.Application.Caching;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Queries.Handlers;

public class GetOfficeByIdQueryHandler : IRequestHandler<GetOfficeByIdQuery, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeResponseFactory _officeResponseFactory;
    private readonly ICacheService _cache;

    public GetOfficeByIdQueryHandler(
        IOfficeRepository officeRepository,
        IOfficeResponseFactory officeResponseFactory,
        ICacheService cache)
    {
        _officeRepository = officeRepository;
        _officeResponseFactory = officeResponseFactory;
        _cache = cache;
    }

    public async Task<OfficeResponse> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
    {
        var version = await _cache.GetVersionAsync(OfficeCache.Scope, cancellationToken);

        return await _cache.GetOrCreateAsync(OfficeCache.ById(version, request.Id), async token =>
        {
            var office = await _officeRepository.GetByIdAsync(request.Id, token)
                         ?? throw new OfficeNotFoundException($"Office with id {request.Id} not found");

            return await _officeResponseFactory.CreateAsync(office, token);
        }, OfficeCache.Ttl, cancellationToken);
    }
}