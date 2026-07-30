using MediatR;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class UpdateSpecializationCommandHandler : IRequestHandler<UpdateSpecializationCommand, Unit>
{
    private readonly ISpecializationRepository _specializationRepository;

    public UpdateSpecializationCommandHandler(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<Unit> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.Id);
        }

        specialization.Name = request.Name;
        specialization.Status = request.Status;
        specialization.UpdatedAt = DateTime.UtcNow;

        await _specializationRepository.UpdateAsync(specialization, cancellationToken);
        
        return Unit.Value;
    }
}