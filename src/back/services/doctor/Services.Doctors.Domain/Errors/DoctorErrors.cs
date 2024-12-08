using Shared.Common.Helper.ErrorsHandler;

namespace Services.Doctors.Domain.Errors;

public sealed class DoctorErrors
{
    public static Error NotFound
        = Error.NotFound("credentialNotFound", "The credential was not found");

    public static Error NotModified
        = Error.BadRequest("credentialNotModified", "The credential was not modified");
}
