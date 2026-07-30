using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;
using Services.Application.DTO_s.Specialization;
using Services.Application.Interfaces;

namespace Services.Application.Queries.Services;

public class GetConsultationsQueryHandler : IRequestHandler<GetConsultationsQuery, PagedResult<SpecializationGroupDto>>
{
    private readonly ISpecializationRepository _specializationRepository;

    public GetConsultationsQueryHandler(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<PagedResult<SpecializationGroupDto>> Handle(GetConsultationsQuery request, CancellationToken cancellationToken)
    {
        var specializations = await _specializationRepository.GetWithConsultationsAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _specializationRepository.GetTotalCountWithConsultationsAsync(cancellationToken);
        
        var items = specializations.Select(s => new SpecializationGroupDto
        {
            Id = s.Id,
            Name = s.Name,
            Services = s.Services.Select(srv => new ServiceDto
            {
                Id = srv.Id,
                Name = srv.Name,
                Price = srv.Price,
                Category = srv.Category,
                Status = srv.Status,
                SpecializationId = srv.SpecializationId
            }).ToList()
        }).ToList();

        return new PagedResult<SpecializationGroupDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
