using MediatR;
using Services.Application.Interfaces;
using Services.Domain.Entities;

namespace Services.Application.Commands.Specializations.CreateSpecialization;

public class CreateSpecializationCommandHandler : IRequestHandler<CreateSpecializationCommand, Guid>
{
    private readonly ISpecializationRepository _specializationRepository;

    public CreateSpecializationCommandHandler(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<Guid> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            Services = request.Services.Select(s => new Service
            {
                Id = Guid.NewGuid(),
                Name = s.Name,
                Price = s.Price,
                Category = s.Category,
                Status = s.Status,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        await _specializationRepository.AddAsync(specialization, cancellationToken);
        
        return specialization.Id;
    }
}