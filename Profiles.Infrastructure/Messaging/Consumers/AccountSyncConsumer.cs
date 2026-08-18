using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Domain.Entities;
using Authorization.Infrastructure.Messaging.Contracts.Events.Accounts;
using Profiles.Application.Interfaces.Repositories;

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

        _logger.LogInformation("Received AccountCreated event: AccountId={AccountId}, Role={Role}", message.Id, message.Role);

        await CreateProfileByRole(message.Id, message.Role, context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<AccountUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received AccountUpdated event: AccountId={AccountId}", message.Id);

        await UpdateProfileByRole(message.Id, message.FirstName, message.LastName, message.MiddleName, context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<AccountDeactivated> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received AccountDeactivated event: AccountId={AccountId}", message.Id);

        await DeleteProfileByAccountId(message.Id, context.CancellationToken);
    }

    private async Task CreateProfileByRole(Guid accountId, string role, CancellationToken cancellationToken)
    {
        switch (role)
        {
            case "Doctor":
                var doctorExists = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);

                if (doctorExists.IsSuccess && doctorExists.Value is not null)
                {
                    _logger.LogWarning("Doctor profile already exists for AccountId: {AccountId}", accountId);

                    return;
                }

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId
                };

                await _doctorRepository.CreateAsync(doctor, cancellationToken);

                _logger.LogInformation("Doctor profile created for AccountId: {AccountId}", accountId);

                break;

            case "Patient":
                var patientExists = await _patientRepository.GetByAccountIdAsync(accountId, cancellationToken);

                if (patientExists.IsSuccess && patientExists.Value is not null)
                {
                    _logger.LogWarning("Patient profile already exists for AccountId: {AccountId}", accountId);

                    return;
                }

                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    IsLinkedToAccount = true
                };

                await _patientRepository.CreateAsync(patient, cancellationToken);

                _logger.LogInformation("Patient profile created for AccountId: {AccountId}", accountId);

                break;

            case "Receptionist":
                var receptionistExists = await _receptionistRepository.GetByAccountIdAsync(accountId, cancellationToken);

                if (receptionistExists.IsSuccess && receptionistExists.Value is not null)
                {
                    _logger.LogWarning("Receptionist profile already exists for AccountId: {AccountId}", accountId);

                    return;
                }

                var receptionist = new Receptionist
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId
                };

                await _receptionistRepository.CreateAsync(receptionist, cancellationToken);

                _logger.LogInformation("Receptionist profile created for AccountId: {AccountId}", accountId);

                break;

            default:
                _logger.LogWarning("Unknown role: {Role} for AccountId: {AccountId}", role, accountId);

                break;
        }
    }

    private async Task UpdateProfileByRole(Guid accountId, string firstName, string lastName, string? middleName, CancellationToken cancellationToken)
    {
        var doctorResult = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (doctorResult.IsSuccess && doctorResult.Value is not null)
        {
            var doctor = doctorResult.Value;

            doctor.FirstName = firstName;
            doctor.LastName = lastName;
            doctor.MiddleName = middleName;

            await _doctorRepository.UpdateAsync(doctor, cancellationToken);

            _logger.LogInformation("Doctor profile updated for AccountId: {AccountId}", accountId);

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

            _logger.LogInformation("Patient profile updated for AccountId: {AccountId}", accountId);

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

            _logger.LogInformation("Receptionist profile updated for AccountId: {AccountId}", accountId);

            return;
        }

        _logger.LogWarning("No profile found for AccountId: {AccountId}", accountId);
    }

    private async Task DeleteProfileByAccountId(Guid accountId, CancellationToken cancellationToken)
    {
        var doctorResult = await _doctorRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (doctorResult.IsSuccess && doctorResult.Value is not null)
        {
            await _doctorRepository.DeleteAsync(doctorResult.Value.Id, cancellationToken);

            _logger.LogInformation("Doctor profile deleted for AccountId: {AccountId}", accountId);

            return;
        }

        var patientResult = await _patientRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (patientResult.IsSuccess && patientResult.Value is not null)
        {
            await _patientRepository.DeleteAsync(patientResult.Value.Id, cancellationToken);

            _logger.LogInformation("Patient profile deleted for AccountId: {AccountId}", accountId);

            return;
        }

        var receptionistResult = await _receptionistRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (receptionistResult.IsSuccess && receptionistResult.Value is not null)
        {
            await _receptionistRepository.DeleteAsync(receptionistResult.Value.Id, cancellationToken);

            _logger.LogInformation("Receptionist profile deleted for AccountId: {AccountId}", accountId);

            return;
        }

        _logger.LogWarning("No profile found for deletion: AccountId: {AccountId}", accountId);
    }
}