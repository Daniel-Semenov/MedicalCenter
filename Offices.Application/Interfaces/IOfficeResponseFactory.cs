using Offices.Application.DTO_s;
using Offices.Domain.Entities;

namespace Offices.Application.Interfaces;

public interface IOfficeResponseFactory
{
    Task<OfficeResponse> CreateAsync(Office office, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OfficeResponse>> CreateManyAsync(
        IReadOnlyList<Office> offices,
        CancellationToken cancellationToken = default);
}