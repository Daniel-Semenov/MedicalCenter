using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;

    public UpdateOfficeCommandHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<OfficeResponse> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (office is null)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }

        office.UpdateDetails(
            request.City,
            request.Street,
            request.HouseNumber,
            request.OfficeNumber,
            request.PhoneNumber);

        await _officeRepository.UpdateAsync(office, cancellationToken);

        return new OfficeResponse
        {
            Id = office.Id,
            FullAddress = $"{office.City}, {office.Street} {office.HouseNumber}/{office.OfficeNumber}",
            IsActive = office.IsActive,
            PhoneNumber = office.PhoneNumber,
            PhotoUrl = office.PhotoUrl
        };
    }
}