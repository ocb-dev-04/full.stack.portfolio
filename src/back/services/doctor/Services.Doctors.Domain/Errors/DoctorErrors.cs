using Shared.Common.Helper.ErrorsHandler;

namespace Services.Doctors.Domain.Errors;

public sealed class DoctorErrors
{
    public static Error NotFound
        = Error.NotFound("credentialNotFound", "The credential was not found");

    public static Error NotModified
        = Error.BadRequest("credentialNotModified", "The credential was not modified");

    public static Error AlreadyExist
        = Error.BadRequest("doctorAlreadyExist", "The doctor already exist");

    public static Error YouAreNotTheOwner
        = Error.BadRequest("youAreNotTheOwner", "You not are the owner");

}
