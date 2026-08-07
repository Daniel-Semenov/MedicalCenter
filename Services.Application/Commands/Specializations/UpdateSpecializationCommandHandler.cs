using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class UpdateSpecializationCommandHandler : IRequestHandler<UpdateSpecializationCommand, Unit>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublisher _publisher;

    public UpdateSpecializationCommandHandler(
        ISpecializationRepository specializationRepository,
        IPublisher publisher)
    {
        _specializationRepository = specializationRepository;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.Id);
        }
        
        var exists = await _specializationRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken);

        if (exists)
        {
            throw new SpecializationAlreadyExistsException(request.Name);
        }

        specialization.Name = request.Name;
        specialization.Status = request.Status;
        specialization.UpdatedAt = DateTime.UtcNow;

        await _specializationRepository.UpdateAsync(specialization, cancellationToken);

        await _publisher.Publish(new SpecializationUpdatedNotification(specialization), cancellationToken);

        await _specializationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}