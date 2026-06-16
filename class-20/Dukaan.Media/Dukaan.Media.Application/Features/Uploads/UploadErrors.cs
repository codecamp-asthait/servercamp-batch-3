using ErrorOr;

namespace Dukaan.Media.Application.Features.Uploads;

public static class UploadErrors
{
    public static Error NotInUploadingState => Error.Conflict("Uploads.NotInUploadingState", "Media is not in uploading state.");
    public static Error IncompleteUpload => Error.Conflict("Uploads.IncompleteUpload", "Upload is incomplete.");
}
