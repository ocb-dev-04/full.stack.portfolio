using Shared.Common.Helper.ErrorsHandler;

namespace Services.Auth.Domain.Errors;

public sealed class DoctorErrors
{
    public static Error NotFound
        = Error.NotFound("doctorNotFound", "The doctor was not found");

    public static Error NotModified
        = Error.BadRequest("doctorNotModified", "The doctor was not modified");
}
