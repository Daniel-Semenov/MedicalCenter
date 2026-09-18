using Authorization.Application.Constants;
using FluentValidation;

namespace Authorization.Application.Commands.Accounts;

public class UpdateAccountPhotoCommandValidator : AbstractValidator<UpdateAccountPhotoCommand>
{
    public UpdateAccountPhotoCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.MaxFileNameLength);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => ct.StartsWith("image/"))
            .WithMessage("File must be an image");

        RuleFor(x => x.PhotoStream)
            .NotNull();
    }
}