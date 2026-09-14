using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;

namespace Offices.Application.Queries.Handlers;

public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, PagedResult<OfficeResponse>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeResponseFactory _officeResponseFactory;

    public GetAllOfficesQueryHandler(
        IOfficeRepository officeRepository,
        IOfficeResponseFactory officeResponseFactory)
    {
        _officeRepository = officeRepository;
        _officeResponseFactory = officeResponseFactory;
    }

    public async Task<PagedResult<OfficeResponse>> Handle(
        GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var allOffices = await _officeRepository.GetAllAsync(cancellationToken);
        var officeList = allOffices.ToList();
        var totalCount = officeList.Count;

        var pagedOffices = officeList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = await _officeResponseFactory.CreateManyAsync(pagedOffices, cancellationToken);

        return new PagedResult<OfficeResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}