using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Application.Interfaces.Services;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<PatientResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        var dto = new PatientResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            IsLinkedToAccount = result.Value.IsLinkedToAccount
        };

        return Result<PatientResponseDto>.Success(dto);
    }

    public async Task<Result<PatientResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        var dto = new PatientResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            IsLinkedToAccount = result.Value.IsLinkedToAccount
        };

        return Result<PatientResponseDto>.Success(dto);
    }

    public async Task<Result<PagedResult<PatientResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetPagedAsync(page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<PatientResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(patient => new PatientResponseDto
        {
            Id = patient.Id,
            AccountId = patient.AccountId,
            IsLinkedToAccount = patient.IsLinkedToAccount
        });

        var pagedResult = new PagedResult<PatientResponseDto>
        {
            Items = items,
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize
        };

        return Result<PagedResult<PatientResponseDto>>.Success(pagedResult);
    }

    public async Task<Result<PatientResponseDto>> CreateAsync(CreatePatientRequestDto request, CancellationToken cancellationToken = default)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            IsLinkedToAccount = request.IsLinkedToAccount
        };

        var result = await _patientRepository.CreateAsync(patient, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        var dto = new PatientResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            IsLinkedToAccount = result.Value.IsLinkedToAccount
        };

        return Result<PatientResponseDto>.Success(dto);
    }

    public async Task<Result<PatientResponseDto>> UpdateAsync(Guid id, UpdatePatientRequestDto request, CancellationToken cancellationToken = default)
    {
        var getResult = await _patientRepository.GetByIdAsync(id, cancellationToken);
        

        if (!getResult.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(getResult.Error);
        }

        var patient = getResult.Value;
        patient.IsLinkedToAccount = request.IsLinkedToAccount;

        var updateResult = await _patientRepository.UpdateAsync(patient, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(updateResult.Error);
        }

        var dto = new PatientResponseDto
        {
            Id = patient.Id,
            AccountId = patient.AccountId,
            IsLinkedToAccount = patient.IsLinkedToAccount
        };

        return Result<PatientResponseDto>.Success(dto);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _patientRepository.DeleteAsync(id, cancellationToken);
    }
}