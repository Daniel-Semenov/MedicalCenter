using MediatR;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class ToggleOfficeStatusCommandHandler : IRequestHandler<ToggleOfficeStatusCommand>
{
    private readonly IOfficeRepository _officeRepository;

    public ToggleOfficeStatusCommandHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task Handle(ToggleOfficeStatusCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (office is null)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }
        
        office.ToggleStatus();
        
        await _officeRepository.UpdateAsync(office, cancellationToken);
    }
}