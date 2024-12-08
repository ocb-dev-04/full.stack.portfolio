using Services.Doctors.Domain.Entities;
using Services.Doctors.Domain.StrongIds;
using Shared.Domain.Abstractions.Repositories;
using Value.Objects.Helper.Values.Primitives;

namespace Services.Doctors.Domain.Abstractions;

public interface IDoctorRepository
    : ISingleQueriesGenericRepository<Doctor, DoctorId>,
        ICreateGenericRepository<Doctor, DoctorId>,
        IDeleteAsyncGenericRepository<Doctor, DoctorId>,
        IDisposable
{
    /// <summary>
    /// Get <see cref="Doctor"/> collection by names
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Doctor>> ByNameAsync(StringObject name, int pageNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken);
}