using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Application.Interfaces.Services;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<Result<DoctorResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(result.Error);
        }

        return Result<DoctorResponseDto>.Success(new DoctorResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            SpecializationId = result.Value.SpecializationId,
            OfficeId = result.Value.OfficeId,
            CareerStartYear = result.Value.CareerStartYear
        });
    }

    public async Task<Result<DoctorResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(result.Error);
        }

        return Result<DoctorResponseDto>.Success(new DoctorResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            SpecializationId = result.Value.SpecializationId,
            OfficeId = result.Value.OfficeId,
            CareerStartYear = result.Value.CareerStartYear
        });
    }

    public async Task<Result<PagedResult<DoctorResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetPagedAsync(page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<DoctorResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(d => new DoctorResponseDto
        {
            Id = d.Id,
            AccountId = d.AccountId,
            SpecializationId = d.SpecializationId,
            OfficeId = d.OfficeId,
            CareerStartYear = d.CareerStartYear
        });

        var pagedResult = new PagedResult<DoctorResponseDto>
        {
            Items = items,
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize
        };

        return Result<PagedResult<DoctorResponseDto>>.Success(pagedResult);
    }

    public async Task<Result<PagedResult<DoctorResponseDto>>> GetBySpecializationAsync(Guid specializationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetBySpecializationPagedAsync(specializationId, page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<DoctorResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(d => new DoctorResponseDto
        {
            Id = d.Id,
            AccountId = d.AccountId,
            SpecializationId = d.SpecializationId,
            OfficeId = d.OfficeId,
            CareerStartYear = d.CareerStartYear
        });

        var pagedResult = new PagedResult<DoctorResponseDto>
        {
            Items = items,
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize
        };

        return Result<PagedResult<DoctorResponseDto>>.Success(pagedResult);
    }

    public async Task<Result<DoctorResponseDto>> CreateAsync(CreateDoctorRequestDto request, CancellationToken cancellationToken = default)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            SpecializationId = request.SpecializationId,
            OfficeId = request.OfficeId,
            CareerStartYear = request.CareerStartYear
        };

        var result = await _doctorRepository.CreateAsync(doctor, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(result.Error);
        }

        return Result<DoctorResponseDto>.Success(new DoctorResponseDto
        {
            Id = result.Value.Id,
            AccountId = result.Value.AccountId,
            SpecializationId = result.Value.SpecializationId,
            OfficeId = result.Value.OfficeId,
            CareerStartYear = result.Value.CareerStartYear
        });
    }

    public async Task<Result<DoctorResponseDto>> UpdateAsync(Guid id, UpdateDoctorRequestDto request, CancellationToken cancellationToken = default)
    {
        var getResult = await _doctorRepository.GetByIdAsync(id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(getResult.Error);
        }

        var doctor = getResult.Value;
        doctor.SpecializationId = request.SpecializationId;
        doctor.OfficeId = request.OfficeId;
        doctor.CareerStartYear = request.CareerStartYear;

        var updateResult = await _doctorRepository.UpdateAsync(doctor, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(updateResult.Error);
        }

        return Result<DoctorResponseDto>.Success(new DoctorResponseDto
        {
            Id = doctor.Id,
            AccountId = doctor.AccountId,
            SpecializationId = doctor.SpecializationId,
            OfficeId = doctor.OfficeId,
            CareerStartYear = doctor.CareerStartYear
        });
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _doctorRepository.DeleteAsync(id, cancellationToken);
    }
}