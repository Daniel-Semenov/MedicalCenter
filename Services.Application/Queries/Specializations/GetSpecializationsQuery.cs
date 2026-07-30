using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Specialization;
using Services.Application.Interfaces;

namespace Services.Application.Queries.Specializations;

public class GetSpecializationsQuery : IRequest<PagedResult<SpecializationDto>>, IPagedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}