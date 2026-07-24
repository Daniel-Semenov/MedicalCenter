using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;

namespace Offices.Application.Queries.Handlers;

public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, PagedResult<OfficeResponse>>
{
    private readonly IOfficeRepository _officeRepository;

    public GetAllOfficesQueryHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<PagedResult<OfficeResponse>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var allOffices = await _officeRepository.GetAllAsync(cancellationToken);
        
        var officeList = allOffices.ToList();
        var totalCount = officeList.Count;

        var pagedOffices = officeList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(office => new OfficeResponse
            {
                Id = office.Id,
                FullAddress = $"{office.City}, {office.Street} {office.HouseNumber}/{office.OfficeNumber}",
                IsActive = office.IsActive,
                PhoneNumber = office.PhoneNumber,
                PhotoUrl = office.PhotoUrl
            });

        return new PagedResult<OfficeResponse>
        {
            Items = pagedOffices,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}