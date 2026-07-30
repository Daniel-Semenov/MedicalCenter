using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;
using Services.Application.Interfaces;

namespace Services.Application.Queries.Services;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, PagedResult<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<PagedResult<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        var totalCount = await _serviceRepository.GetTotalCountAsync(cancellationToken);
        
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
            Page = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
