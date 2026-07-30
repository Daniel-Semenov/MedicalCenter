using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Specialization;

namespace Services.Application.Queries.Specializations;

public class GetSpecializationsQuery : IRequest<PagedResult<SpecializationDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
