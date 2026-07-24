using FluentValidation;
using Offices.Application.Commands;
using Offices.Domain.Constants;

namespace Offices.Application.Validators;

public class UpdateOfficePhotoCommandValidator : AbstractValidator<UpdateOfficePhotoCommand>
{
    public UpdateOfficePhotoCommandValidator()
    {
        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("OfficeId is required");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required")
            .Must(ct => ct.StartsWith("image/")).WithMessage("File must be an image");

        RuleFor(x => x.PhotoStream)
            .NotNull().WithMessage("PhotoStream is required");

        RuleFor(x => x.FileName)
            .MaximumLength(ValidationConstants.MaxFileNameLength).WithMessage($"File name must not exceed {ValidationConstants.MaxFileNameLength} characters");
    }
}