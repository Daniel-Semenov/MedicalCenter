using MediatR;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class DeleteSpecializationCommandHandler : IRequestHandler<DeleteSpecializationCommand, Unit>
{
    private readonly ISpecializationRepository _specializationRepository;

    public DeleteSpecializationCommandHandler(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<Unit> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.Id);
        }

        await _specializationRepository.DeleteAsync(specialization, cancellationToken);

        return Unit.Value;
    }
}