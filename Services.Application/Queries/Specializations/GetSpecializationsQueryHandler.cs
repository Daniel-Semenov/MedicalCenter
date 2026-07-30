using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;
using Services.Application.DTO_s.Specialization;
using Services.Application.Interfaces;

namespace Services.Application.Queries.Specializations;

public class GetSpecializationsQueryHandler : IRequestHandler<GetSpecializationsQuery, PagedResult<SpecializationDto>>
{
    private readonly ISpecializationRepository _specializationRepository;

    public GetSpecializationsQueryHandler(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<PagedResult<SpecializationDto>> Handle(GetSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var specializations = await _specializationRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _specializationRepository.GetTotalCountAsync(cancellationToken);
        
        var items = specializations.Select(s => new SpecializationDto
        {
            Id = s.Id,
            Name = s.Name,
            Status = s.Status,
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

        return new PagedResult<SpecializationDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
