using MediatR;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public CreateServiceCommandHandler(
        IServiceRepository serviceRepository,
        ISpecializationRepository specializationRepository)
    {
        _serviceRepository = serviceRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.SpecializationId);
        }

        var service = new Service
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            Status = request.Status,
            SpecializationId = request.SpecializationId,
            CreatedAt = DateTime.UtcNow
        };

        await _serviceRepository.AddAsync(service, cancellationToken);

        return service.Id;
    }
}