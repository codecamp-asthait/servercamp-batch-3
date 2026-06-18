using ErrorOr;

namespace Dukaan.Media.Application.Features.Media;

public static class MediaErrors
{
    public static Error NotFound => Error.NotFound("Media.NotFound", "Media not found.");
    public static Error VariantNotFound => Error.NotFound("Media.VariantNotFound", "Media variant not found.");
}
