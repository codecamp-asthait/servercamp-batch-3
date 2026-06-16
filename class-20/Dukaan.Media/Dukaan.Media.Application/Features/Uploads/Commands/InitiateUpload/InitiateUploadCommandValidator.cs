using FluentValidation;

namespace Dukaan.Media.Application.Features.Uploads.Commands.InitiateUpload;

public class InitiateUploadCommandValidator : AbstractValidator<InitiateUploadCommand>
{
    public InitiateUploadCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ContentType)
            .Must(ct => ct.StartsWith("image/"))
            .WithMessage("Only image files are allowed.");

        RuleFor(x => x.TotalFileSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100 * 1024 * 1024)
            .WithMessage("File must be between 1 byte and 100 MB.");
    }
}
