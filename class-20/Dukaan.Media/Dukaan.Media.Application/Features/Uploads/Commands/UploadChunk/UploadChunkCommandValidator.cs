using FluentValidation;

namespace Dukaan.Media.Application.Features.Uploads.Commands.UploadChunk;

public class UploadChunkCommandValidator : AbstractValidator<UploadChunkCommand>
{
    private const long MaxChunkSize = 5 * 1024 * 1024 + 1024;

    public UploadChunkCommandValidator()
    {
        RuleFor(x => x.MediaId).NotEmpty();
        RuleFor(x => x.ChunkIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ChunkStream).NotNull().WithMessage("Chunk is required.");
        RuleFor(x => x.ChunkLength)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxChunkSize)
            .WithMessage("Chunk must not exceed 5 MB.");
    }
}
