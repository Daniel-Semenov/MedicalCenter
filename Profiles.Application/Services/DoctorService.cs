using MediatR;
using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Application.Interfaces;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Application.Interfaces.Services;
using Profiles.Application.Notifications.Doctors;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMediator _mediator;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IMediator mediator)
    {
        _doctorRepository = doctorRepository;
        _mediator = mediator;
    }

    public async Task<Result<DoctorResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(result.Error);
        }

        return Result<DoctorResponseDto>.Success(MapToDto(result.Value));
    }

    public async Task<Result<DoctorResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(result.Error);
        }

        return Result<DoctorResponseDto>.Success(MapToDto(result.Value));
    }

    public async Task<Result<PagedResult<DoctorResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _doctorRepository.GetPagedAsync(page, pageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<DoctorResponseDto>>.Failure(result.Error);
        }

        var items = result.Value.Items.Select(MapToDto);

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

        var items = result.Value.Items.Select(MapToDto);

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
        var doctorId = Guid.NewGuid();

        var doctor = new Doctor
        {
            Id = doctorId,
            AccountId = request.AccountId,
            SpecializationId = request.SpecializationId,
            OfficeId = request.OfficeId,
            CareerStartYear = request.CareerStartYear,
            PhotoUrl = request.PhotoUrl,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            OfficeName = request.OfficeName,
            OfficeAddress = request.OfficeAddress,
            SpecializationName = request.SpecializationName,
            Services = request.Services.Select(s => new Domain.Entities.DoctorService
            {
                Id = Guid.NewGuid(),
                DoctorId = doctorId,
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName
            }).ToList()
        };

        var result = await _doctorRepository.CreateAsync(doctor, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(result.Error);
        }

        await _mediator.Publish(new DoctorCreatedNotification(
            result.Value.Id,
            result.Value.AccountId,
            result.Value.SpecializationId,
            result.Value.OfficeId,
            result.Value.CareerStartYear,
            DateTime.UtcNow
        ), cancellationToken);

        return Result<DoctorResponseDto>.Success(MapToDto(result.Value));
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
        doctor.PhotoUrl = request.PhotoUrl;
        doctor.FirstName = request.FirstName;
        doctor.LastName = request.LastName;
        doctor.MiddleName = request.MiddleName;
        doctor.OfficeName = request.OfficeName;
        doctor.OfficeAddress = request.OfficeAddress;
        doctor.SpecializationName = request.SpecializationName;
        doctor.Services = request.Services.Select(s => new Domain.Entities.DoctorService
        {
            Id = Guid.NewGuid(),
            DoctorId = doctor.Id,
            ServiceId = s.ServiceId,
            ServiceName = s.ServiceName
        }).ToList();

        var updateResult = await _doctorRepository.UpdateAsync(doctor, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result<DoctorResponseDto>.Failure(updateResult.Error);
        }

        await _mediator.Publish(new DoctorUpdatedNotification(
            doctor.Id,
            doctor.AccountId,
            doctor.SpecializationId,
            doctor.OfficeId,
            doctor.CareerStartYear,
            DateTime.UtcNow
        ), cancellationToken);

        return Result<DoctorResponseDto>.Success(MapToDto(doctor));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _doctorRepository.DeleteAsync(id, cancellationToken);

        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        await _mediator.Publish(new DoctorDeletedNotification(
            id,
            DateTime.UtcNow
        ), cancellationToken);

        return deleteResult;
    }

    private static DoctorResponseDto MapToDto(Doctor doctor) => new()
    {
        Id = doctor.Id,
        AccountId = doctor.AccountId,
        SpecializationId = doctor.SpecializationId,
        OfficeId = doctor.OfficeId,
        CareerStartYear = doctor.CareerStartYear,
        Experience = DateTime.UtcNow.Year - doctor.CareerStartYear,
        PhotoUrl = doctor.PhotoUrl,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName,
        MiddleName = doctor.MiddleName,
        OfficeName = doctor.OfficeName,
        OfficeAddress = doctor.OfficeAddress,
        SpecializationName = doctor.SpecializationName,
        Services = doctor.Services.Select(s => new DoctorServiceResponseDto
        {
            ServiceId = s.ServiceId,
            ServiceName = s.ServiceName
        }).ToList()
    };
}