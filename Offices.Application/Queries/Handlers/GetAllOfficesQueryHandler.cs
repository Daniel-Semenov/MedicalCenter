using MediatR;
using Offices.Application.Caching;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;

namespace Offices.Application.Queries.Handlers;

public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, PagedResult<OfficeResponse>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeResponseFactory _officeResponseFactory;
    private readonly ICacheService _cache;

    public GetAllOfficesQueryHandler(
        IOfficeRepository officeRepository,
        IOfficeResponseFactory officeResponseFactory,
        ICacheService cache)
    {
        _officeRepository = officeRepository;
        _officeResponseFactory = officeResponseFactory;
        _cache = cache;
    }

    public async Task<PagedResult<OfficeResponse>> Handle(
        GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var version = await _cache.GetVersionAsync(OfficeCache.Scope, cancellationToken);
        var key = OfficeCache.List(version, request.Page, request.PageSize);

        return await _cache.GetOrCreateAsync(key, async token =>
        {
            var (offices, totalCount) = await _officeRepository.GetPagedAsync(
                request.Page, request.PageSize, token);

            var items = await _officeResponseFactory.CreateManyAsync(offices, token);

            return new PagedResult<OfficeResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }, OfficeCache.Ttl, cancellationToken);
    }
}