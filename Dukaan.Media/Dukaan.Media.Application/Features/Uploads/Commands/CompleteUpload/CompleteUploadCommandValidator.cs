using FluentValidation;

namespace Dukaan.Media.Application.Features.Uploads.Commands.CompleteUpload;

public class CompleteUploadCommandValidator : AbstractValidator<CompleteUploadCommand>
{
    public CompleteUploadCommandValidator()
    {
        RuleFor(x => x.MediaId).NotEmpty();
    }
}
