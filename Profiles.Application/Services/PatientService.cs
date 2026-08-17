using MediatR;
using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Application.Interfaces.Services;
using Profiles.Application.Notifications.Patients;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMediator _mediator;

    public PatientService(
        IPatientRepository patientRepository,
        IMediator mediator)
    {
        _patientRepository = patientRepository;
        _mediator = mediator;
    }

    public async Task<Result<PatientResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        return Result<PatientResponseDto>.Success(MapToDto(result.Value));
    }
    
    public async Task<Result<PatientResponseDto>> GetByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetByNameAsync(searchTerm, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        return Result<PatientResponseDto>.Success(MapToDto(result.Value));
    }

    public async Task<Result<PatientResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        return Result<PatientResponseDto>.Success(MapToDto(result.Value));
    }

    public async Task<Result<PagedResult<PatientResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _patientRepository.GetPagedAsync(page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<PatientResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(MapToDto);

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
            IsLinkedToAccount = request.IsLinkedToAccount,
            PhotoUrl = request.PhotoUrl,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth
        };

        var result = await _patientRepository.CreateAsync(patient, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(result.Error);
        }

        await _mediator.Publish(new PatientCreatedNotification(
            result.Value.Id,
            result.Value.AccountId,
            result.Value.IsLinkedToAccount,
            DateTime.UtcNow
        ), cancellationToken);

        return Result<PatientResponseDto>.Success(MapToDto(result.Value));
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
        patient.PhotoUrl = request.PhotoUrl;
        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;
        patient.MiddleName = request.MiddleName;
        patient.PhoneNumber = request.PhoneNumber;
        patient.DateOfBirth = request.DateOfBirth;

        var updateResult = await _patientRepository.UpdateAsync(patient, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result<PatientResponseDto>.Failure(updateResult.Error);
        }

        await _mediator.Publish(new PatientUpdatedNotification(
            patient.Id,
            patient.AccountId,
            patient.IsLinkedToAccount,
            DateTime.UtcNow
        ), cancellationToken);

        return Result<PatientResponseDto>.Success(MapToDto(patient));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _patientRepository.DeleteAsync(id, cancellationToken);

        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        await _mediator.Publish(new PatientDeletedNotification(
            id,
            DateTime.UtcNow
        ), cancellationToken);

        return deleteResult;
    }

    private static PatientResponseDto MapToDto(Patient patient) => new()
    {
        Id = patient.Id,
        AccountId = patient.AccountId,
        IsLinkedToAccount = patient.IsLinkedToAccount,
        PhotoUrl = patient.PhotoUrl,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        MiddleName = patient.MiddleName,
        PhoneNumber = patient.PhoneNumber,
        DateOfBirth = patient.DateOfBirth
    };
}