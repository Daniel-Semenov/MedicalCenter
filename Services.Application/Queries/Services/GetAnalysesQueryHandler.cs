using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;
using Services.Application.Interfaces;
using Services.Domain.Enums;

namespace Services.Application.Queries.Services;

public class GetAnalysesQueryHandler : IRequestHandler<GetAnalysesQuery, PagedResult<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAnalysesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<PagedResult<ServiceDto>> Handle(GetAnalysesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetByCategoryAsync(ServiceCategory.Analyses, request.Page, request.PageSize, cancellationToken);
        var totalCount = await _serviceRepository.GetTotalCountByCategoryAsync(ServiceCategory.Analyses, cancellationToken);
        
        var items = services.Select(s => new ServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price,
            Category = s.Category,
            Status = s.Status,
            SpecializationId = s.SpecializationId
        }).ToList();

        return new PagedResult<ServiceDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
