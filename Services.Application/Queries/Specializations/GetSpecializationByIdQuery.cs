using MediatR;
using Services.Application.DTO_s.Specialization;

namespace Services.Application.Queries.Specializations;

public class GetSpecializationByIdQuery : IRequest<SpecializationDto>
{
    public Guid Id { get; set; }
}