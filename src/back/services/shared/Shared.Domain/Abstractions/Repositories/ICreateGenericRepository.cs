using Value.Objects.Helper.Abstractions;

namespace Shared.Domain.Abstractions.Repositories;

public interface ICreateGenericRepository<TEntity, TId> 
    where TEntity : class
    where TId : BaseId
{
    /// <summary>
    /// Create a new <see cref="TEntity"/>
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateAsync(TEntity model, CancellationToken cancellationToken);
}