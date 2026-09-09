using Documents.Application.Commands;
using Documents.Application.Options;
using Documents.Shared.Common.Constants;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Documents.Application.Validators;

public class UploadDocumentCommandValidator 
    : AbstractValidator<UploadDocumentCommand>
{
    private readonly ApplicationOptions _options;

    public UploadDocumentCommandValidator(
        IOptions<ApplicationOptions> options)
    {
        _options = options.Value; 

        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Document content is required.");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0)
            .WithMessage("Document size must be greater than zero.")
            .LessThanOrEqualTo(_options.MaxFileSizeBytes)
            .WithMessage($"File size exceeds maximum allowed size of {_options.MaxFileSizeBytes / (1024 * 1024)} MB.");

        RuleFor(x => x.FileName)
            .NotNull()
            .NotEmpty()
            .WithMessage("File name is required.")
            .MaximumLength(ValidationConstants.MaxFileNameLength)
            .WithMessage("File name must not exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotNull()
            .NotEmpty()
            .WithMessage("Content type is required.")
            .Must(BeAllowedContentType)
            .WithMessage($"Content type is not allowed. Allowed types: {string.Join(", ", _options.AllowedContentTypes)}");

        RuleFor(x => x.FileName)
            .Must(HaveAllowedExtension)
            .WithMessage($"File extension is not allowed. Allowed extensions: {string.Join(", ", _options.AllowedExtensions)}");

        RuleFor(x => x.OwnerId)
            .NotEqual(Guid.Empty)
            .WithMessage("Owner ID is required.");

        RuleFor(x => x.OwnerType)
            .IsInEnum()
            .WithMessage("Invalid owner type.");
    }

    private bool BeAllowedContentType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return false;
        }

        if (_options.AllowedContentTypes is null || _options.AllowedContentTypes.Length == 0)
        {
            return true;
        }

        return _options.AllowedContentTypes.Contains(contentType);
    }

    private bool HaveAllowedExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return false;
        }

        if (_options.AllowedExtensions is null || _options.AllowedExtensions.Length == 0)
        {
            return true;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _options.AllowedExtensions.Contains(extension);
    }
}