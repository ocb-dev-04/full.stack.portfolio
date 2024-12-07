using Services.Auth.Domain.Entities;
using Services.Auth.Domain.StrongIds;
using Shared.Common.Helper.ErrorsHandler;
using Value.Objects.Helper.Values.Domain;
using Shared.Domain.Abstractions.Repositories;

namespace Services.Auth.Domain.Abstractions;

public interface IDoctorRepository
    : ISingleQueriesGenericRepository<Doctor, DoctorId>,
        ICreateGenericRepository<Doctor, DoctorId>,
        IDeleteAsyncGenericRepository<Doctor, DoctorId>,
        IDisposable
{
    /// <summary>
    /// Get <see cref="Doctor"/> by <see cref="EmailAddress"/>
    /// </summary>
    /// <param name="email"></param>
    /// <param name="tracking"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<Doctor>> ByIdAsync(EmailAddress email, bool tracking = true, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken);
}