using MediatR;
using Services.Application.DTO_s.Specialization;

namespace Services.Application.Queries.Specializations;

public class GetSpecializationsQuery : IRequest<List<SpecializationDto>>
{
}