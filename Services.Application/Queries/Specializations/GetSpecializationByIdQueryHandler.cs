using MediatR;
using Services.Application.DTO_s.Service;
using Services.Application.DTO_s.Specialization;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Queries.Specializations;

public class GetSpecializationByIdQueryHandler : IRequestHandler<GetSpecializationByIdQuery, SpecializationDto>
{
    private readonly ISpecializationRepository _specializationRepository;

    public GetSpecializationByIdQueryHandler(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<SpecializationDto> Handle(GetSpecializationByIdQuery request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
            throw new NotFoundException(nameof(Specialization), request.Id);

        return new SpecializationDto
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Status = specialization.Status,
            Services = specialization.Services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                Category = s.Category,
                Status = s.Status,
                SpecializationId = s.SpecializationId
            }).ToList()
        };
    }
}