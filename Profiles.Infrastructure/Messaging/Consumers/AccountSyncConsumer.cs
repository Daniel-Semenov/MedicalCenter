using MassTransit;
using Microsoft.Extensions.Logging;
using Messaging.Contracts.Events.Accounts;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Messaging.Consumers;

public class AccountSyncConsumer :
    IConsumer<AccountCreated>,
    IConsumer<AccountUpdated>,
    IConsumer<AccountDeactivated>
{
    private readonly ILogger<AccountSyncConsumer> _logger;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IReceptionistRepository _receptionistRepository;

    public AccountSyncConsumer(
        ILogger<AccountSyncConsumer> logger,
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IReceptionistRepository receptionistRepository)
    {
        _logger = logger;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _receptionistRepository = receptionistRepository;
    }

    public async Task Consume(ConsumeContext<AccountCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received AccountCreated event: AccountId={AccountId}, Role={Role}",
            message.Id,
            message.Role);

        await CreateOrUpdateProfileByRole(
            message.Id,
            message.Email,
            message.Role,
            message.FirstName,
            message.LastName,
            message.MiddleName,
            context.CancellationToken);

        await _doctorRepository.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<AccountUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received AccountUpdated event: AccountId={AccountId}",
            message.Id);

        await UpdateProfileByAccountId(
            message.Id,
            message.Email,
            message.FirstName,
            message.LastName,
            message.MiddleName,
            context.CancellationToken);

        await _doctorRepository.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<AccountDeactivated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received AccountDeactivated event: AccountId={AccountId}",
            message.Id);

        await DeactivateProfileByAccountId(
            message.Id,
            context.CancellationToken);

        await _doctorRepository.SaveChangesAsync(context.CancellationToken);
    }

    private async Task CreateOrUpdateProfileByRole(
        Guid accountId,
        string email,
        string role,
        string firstName,
        string lastName,
        string? middleName,
        CancellationToken cancellationToken)
    {
        switch (role)
        {
            case "Doctor":
            {
                var existingResult = await _doctorRepository.GetByAccountIdAsync(
                    accountId,
                    cancellationToken);

                if (existingResult.IsSuccess && existingResult.Value is not null)
                {
                    var doctor = existingResult.Value;
                    doctor.FirstName = firstName;
                    doctor.LastName = lastName;
                    doctor.MiddleName = middleName;
                    
                    await _doctorRepository.UpdateAsync(doctor, cancellationToken);
                    
                    _logger.LogInformation(
                        "Doctor profile updated for AccountId: {AccountId}",
                        accountId);
                }
                else
                {
                    var doctor = new Doctor
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        FirstName = firstName,
                        LastName = lastName,
                        MiddleName = middleName,
                        CareerStartYear = DateTime.Now.Year,
                        IsActive = true
                    };

                    await _doctorRepository.CreateAsync(doctor, cancellationToken);

                    _logger.LogInformation(
                        "Doctor profile created for AccountId: {AccountId}",
                        accountId);
                }

                break;
            }

            case "Patient":
            {
                var existingResult = await _patientRepository.GetByAccountIdAsync(
                    accountId,
                    cancellationToken);

                if (existingResult.IsSuccess && existingResult.Value is not null)
                {
                    var patient = existingResult.Value;
                    patient.FirstName = firstName;
                    patient.LastName = lastName;
                    patient.MiddleName = middleName;
                    
                    await _patientRepository.UpdateAsync(patient, cancellationToken);
                    
                    _logger.LogInformation(
                        "Patient profile updated for AccountId: {AccountId}",
                        accountId);
                }
                else
                {
                    var patient = new Patient
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        FirstName = firstName,
                        LastName = lastName,
                        MiddleName = middleName,
                        IsLinkedToAccount = true,
                        IsActive = true
                    };

                    await _patientRepository.CreateAsync(patient, cancellationToken);

                    _logger.LogInformation(
                        "Patient profile created for AccountId: {AccountId}",
                        accountId);
                }

                break;
            }

            case "Receptionist":
            {
                var existingResult = await _receptionistRepository.GetByAccountIdAsync(
                    accountId,
                    cancellationToken);

                if (existingResult.IsSuccess && existingResult.Value is not null)
                {
                    var receptionist = existingResult.Value;
                    receptionist.FirstName = firstName;
                    receptionist.LastName = lastName;
                    receptionist.MiddleName = middleName;
                    
                    await _receptionistRepository.UpdateAsync(receptionist, cancellationToken);
                    
                    _logger.LogInformation(
                        "Receptionist profile updated for AccountId: {AccountId}",
                        accountId);
                }
                else
                {
                    var receptionist = new Receptionist
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        FirstName = firstName,
                        LastName = lastName,
                        MiddleName = middleName,
                        IsActive = true
                    };

                    await _receptionistRepository.CreateAsync(receptionist, cancellationToken);

                    _logger.LogInformation(
                        "Receptionist profile created for AccountId: {AccountId}",
                        accountId);
                }

                break;
            }

            default:
                _logger.LogWarning(
                    "Unknown role: {Role} for AccountId: {AccountId}",
                    role,
                    accountId);
                break;
        }
    }

    private async Task UpdateProfileByAccountId(
        Guid accountId,
        string email,
        string firstName,
        string lastName,
        string? middleName,
        CancellationToken cancellationToken)
    {
        var doctorResult = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);
        if (doctorResult.IsSuccess && doctorResult.Value is not null)
        {
            var doctor = doctorResult.Value;
            doctor.FirstName = firstName;
            doctor.LastName = lastName;
            doctor.MiddleName = middleName;

            await _doctorRepository.UpdateAsync(doctor, cancellationToken);

            _logger.LogInformation(
                "Doctor profile updated for AccountId: {AccountId}",
                accountId);
            return;
        }

        var patientResult = await _patientRepository.GetByAccountIdAsync(accountId, cancellationToken);
        if (patientResult.IsSuccess && patientResult.Value is not null)
        {
            var patient = patientResult.Value;
            patient.FirstName = firstName;
            patient.LastName = lastName;
            patient.MiddleName = middleName;

            await _patientRepository.UpdateAsync(patient, cancellationToken);

            _logger.LogInformation(
                "Patient profile updated for AccountId: {AccountId}",
                accountId);
            return;
        }

        var receptionistResult = await _receptionistRepository.GetByAccountIdAsync(accountId, cancellationToken);
        if (receptionistResult.IsSuccess && receptionistResult.Value is not null)
        {
            var receptionist = receptionistResult.Value;
            receptionist.FirstName = firstName;
            receptionist.LastName = lastName;
            receptionist.MiddleName = middleName;

            await _receptionistRepository.UpdateAsync(receptionist, cancellationToken);

            _logger.LogInformation(
                "Receptionist profile updated for AccountId: {AccountId}",
                accountId);
            return;
        }

        _logger.LogWarning(
            "No profile found for update: AccountId: {AccountId}",
            accountId);
    }

    private async Task DeactivateProfileByAccountId(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var doctorResult = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);
        if (doctorResult.IsSuccess && doctorResult.Value is not null)
        {
            var doctor = doctorResult.Value;
            doctor.IsActive = false;
            
            await _doctorRepository.UpdateAsync(doctor, cancellationToken);

            _logger.LogInformation(
                "Doctor profile deactivated for AccountId: {AccountId}",
                accountId);
            return;
        }

        var patientResult = await _patientRepository.GetByAccountIdAsync(accountId, cancellationToken);
        if (patientResult.IsSuccess && patientResult.Value is not null)
        {
            var patient = patientResult.Value;
            patient.IsActive = false;
            
            await _patientRepository.UpdateAsync(patient, cancellationToken);

            _logger.LogInformation(
                "Patient profile deactivated for AccountId: {AccountId}",
                accountId);
            return;
        }

        var receptionistResult = await _receptionistRepository.GetByAccountIdAsync(accountId, cancellationToken);
        if (receptionistResult.IsSuccess && receptionistResult.Value is not null)
        {
            var receptionist = receptionistResult.Value;
            receptionist.IsActive = false;
            
            await _receptionistRepository.UpdateAsync(receptionist, cancellationToken);

            _logger.LogInformation(
                "Receptionist profile deactivated for AccountId: {AccountId}",
                accountId);
            return;
        }

        _logger.LogWarning(
            "No profile found for deactivation: AccountId: {AccountId}",
            accountId);
    }
}