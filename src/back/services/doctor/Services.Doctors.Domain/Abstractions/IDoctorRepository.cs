using Services.Doctors.Domain.Entities;
using Services.Doctors.Domain.StrongIds;
using Value.Objects.Helper.Values.Primitives;
using Shared.Domain.Abstractions.Repositories;
using Shared.Common.Helper.ErrorsHandler;

namespace Services.Doctors.Domain.Abstractions;

public interface IDoctorRepository
    : ISingleQueriesGenericRepository<Doctor, DoctorId>,
        IBooleanGenericRepository<Doctor, DoctorId>,
        ICreateGenericRepository<Doctor, DoctorId>,
        IDeleteAsyncGenericRepository<Doctor, DoctorId>,
        IDisposable
{
    /// <summary>
    /// Get <see cref="Doctor"/> by credential id
    /// </summary>
    /// <param name="credentialId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<Doctor>> ByCredentialId(GuidObject credentialId, CancellationToken cancellationToken);

    /// <summary>
    /// Get <see cref="Doctor"/> collection by names
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Doctor>> CollectionByNameAsync(StringObject name, int pageNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get <see cref="Doctor"/> collection by specialty
    /// </summary>
    /// <param name="specialty"></param>
    /// <param name="pageNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Doctor>> CollectionBySpecialtyAsync(StringObject specialty, int pageNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get specialty collection
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<StringObject>> SpecialtyCollectionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken);
}