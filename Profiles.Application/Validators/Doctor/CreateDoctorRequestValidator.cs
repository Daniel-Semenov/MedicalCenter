using FluentValidation;
using Profiles.Application.DTO_s.Request;
using Profiles.Shared.Common.Constants;

namespace Profiles.Application.Validators.Doctor;

public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequestDto>
{
    public CreateDoctorRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.SpecializationId)
            .NotEmpty();

        RuleFor(x => x.OfficeId)
            .NotEmpty();

        RuleFor(x => x.CareerStartYear)
            .InclusiveBetween(CareerConstants.MinCareerStartYear, DateTime.UtcNow.Year);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.MiddleName)
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.OfficeName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.OfficeNameMaxLength);

        RuleFor(x => x.OfficeAddress)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.OfficeAddressMaxLength);

        RuleFor(x => x.SpecializationName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.SpecializationNameMaxLength);

        RuleForEach(x => x.Services)
            .ChildRules(service =>
            {
                service.RuleFor(s => s.ServiceId)
                    .NotEmpty();

                service.RuleFor(s => s.ServiceName)
                    .NotEmpty()
                    .MaximumLength(FieldLengthConstants.ServiceNameMaxLength);
            });
    }
}