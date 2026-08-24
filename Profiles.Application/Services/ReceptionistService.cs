using MassTransit;
using Messaging.Contracts.Events.Receptionist;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public ReceptionistService(
        IReceptionistRepository receptionistRepository,
        IPublishEndpoint publishEndpoint)
    {
        _receptionistRepository = receptionistRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<ReceptionistResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _receptionistRepository.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(result.Error);
        }

        return Result<ReceptionistResponseDto>.Success(MapToDto(result.Value));
    }

    public async Task<Result<ReceptionistResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = await _receptionistRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(result.Error);
        }

        return Result<ReceptionistResponseDto>.Success(MapToDto(result.Value));
    }

    public async Task<Result<PagedResult<ReceptionistResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _receptionistRepository.GetPagedAsync(page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<ReceptionistResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(MapToDto);

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
            OfficeId = request.OfficeId,
            PhotoUrl = request.PhotoUrl,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            OfficeName = request.OfficeName,
            OfficeAddress = request.OfficeAddress
        };

        var result = await _receptionistRepository.CreateAsync(receptionist, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(result.Error);
        }

        var integrationEvent = new ReceptionistCreated
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            OfficeId = result.Value.OfficeId,
            CreatedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        var saveResult = await _receptionistRepository.SaveChangesAsync(cancellationToken);
        
        if (!saveResult.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(saveResult.Error);
        }

        return Result<ReceptionistResponseDto>.Success(MapToDto(result.Value));
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
        receptionist.PhotoUrl = request.PhotoUrl;
        receptionist.FirstName = request.FirstName;
        receptionist.LastName = request.LastName;
        receptionist.MiddleName = request.MiddleName;
        receptionist.Email = request.Email;
        receptionist.OfficeName = request.OfficeName;
        receptionist.OfficeAddress = request.OfficeAddress;

        var updateResult = await _receptionistRepository.UpdateAsync(receptionist, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(updateResult.Error);
        }

        var integrationEvent = new ReceptionistUpdated
        {
            Id = receptionist.Id,
            AccountId = receptionist.AccountId,
            OfficeId = receptionist.OfficeId,
            UpdatedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        var saveResult = await _receptionistRepository.SaveChangesAsync(cancellationToken);
        
        if (!saveResult.IsSuccess)
        {
            return Result<ReceptionistResponseDto>.Failure(saveResult.Error);
        }

        return Result<ReceptionistResponseDto>.Success(MapToDto(receptionist));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _receptionistRepository.DeleteAsync(id, cancellationToken);

        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var integrationEvent = new ReceptionistDeleted
        {
            Id = id,
            DeletedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        var saveResult = await _receptionistRepository.SaveChangesAsync(cancellationToken);
        
        if (!saveResult.IsSuccess)
        {
            return saveResult;
        }

        return deleteResult;
    }

    private static ReceptionistResponseDto MapToDto(Receptionist receptionist) => new()
    {
        Id = receptionist.Id,
        AccountId = receptionist.AccountId,
        OfficeId = receptionist.OfficeId,
        PhotoUrl = receptionist.PhotoUrl,
        FirstName = receptionist.FirstName,
        LastName = receptionist.LastName,
        MiddleName = receptionist.MiddleName,
        Email = receptionist.Email,
        OfficeName = receptionist.OfficeName,
        OfficeAddress = receptionist.OfficeAddress
    };
}