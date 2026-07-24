using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;

namespace Offices.Application.Queries.Handlers;

public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, IEnumerable<OfficeResponse>>
{
    private readonly IOfficeRepository _officeRepository;

    public GetAllOfficesQueryHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<IEnumerable<OfficeResponse>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var offices = await _officeRepository.GetAllAsync(cancellationToken);

        return offices.Select(office => new OfficeResponse
        {
            Id = office.Id,
            FullAddress = $"{office.City}, {office.Street} {office.HouseNumber}/{office.OfficeNumber}",
            IsActive = office.IsActive,
            PhoneNumber = office.PhoneNumber,
            PhotoUrl = office.PhotoUrl
        });
    }
}