using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Application.Interfaces.Services;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Services;

public class ReceptionistService : IReceptionistService
{
    private readonly IReceptionistRepository _receptionistRepository;

    public ReceptionistService(IReceptionistRepository receptionistRepository)
    {
        _receptionistRepository = receptionistRepository;
    }

    public async Task<Result<ReceptionistResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _receptionistRepository.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(result.Error);
        }

        var dto = new ReceptionistResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            OfficeId = result.Value.OfficeId
        };

        return Result<ReceptionistResponseDto>.Success(dto);
    }

    public async Task<Result<ReceptionistResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = await _receptionistRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(result.Error);
        }

        var dto = new ReceptionistResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            OfficeId = result.Value.OfficeId
        };

        return Result<ReceptionistResponseDto>.Success(dto);
    }

    public async Task<Result<PagedResult<ReceptionistResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _receptionistRepository.GetPagedAsync(page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<ReceptionistResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(receptionist => new ReceptionistResponseDto
        {
            Id = receptionist.Id,
            AccountId = receptionist.AccountId,
            OfficeId = receptionist.OfficeId
        });

        var pagedResult = new PagedResult<ReceptionistResponseDto>
        {
            Items = items,
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize
        };

        return Result<PagedResult<ReceptionistResponseDto>>.Success(pagedResult);
    }

    public async Task<Result<ReceptionistResponseDto>> CreateAsync(CreateReceptionistRequestDto request, CancellationToken cancellationToken = default)
    {
        var receptionist = new Receptionist
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            OfficeId = request.OfficeId
        };

        var result = await _receptionistRepository.CreateAsync(receptionist, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(result.Error);
        }

        var dto = new ReceptionistResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            OfficeId = result.Value.OfficeId
        };

        return Result<ReceptionistResponseDto>.Success(dto);
    }

    public async Task<Result<ReceptionistResponseDto>> UpdateAsync(Guid id, UpdateReceptionistRequestDto request, CancellationToken cancellationToken = default)
    {
        var getResult = await _receptionistRepository.GetByIdAsync(id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(getResult.Error);
        }

        var receptionist = getResult.Value;
        receptionist.OfficeId = request.OfficeId;

        var updateResult = await _receptionistRepository.UpdateAsync(receptionist, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(updateResult.Error);
        }

        var dto = new ReceptionistResponseDto
        {
            Id = receptionist.Id,
            AccountId = receptionist.AccountId,
            OfficeId = receptionist.OfficeId
        };

        return Result<ReceptionistResponseDto>.Success(dto);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _receptionistRepository.DeleteAsync(id, cancellationToken);
    }
}