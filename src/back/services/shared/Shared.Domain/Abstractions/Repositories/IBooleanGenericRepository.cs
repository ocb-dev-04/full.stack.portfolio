using System.Linq.Expressions;

using Value.Objects.Helper.Abstractions;

namespace Shared.Domain.Abstractions.Repositories;

public interface IBooleanGenericRepository<TEntity, TId>
        where TEntity : class
        where TId : BaseId
{
    /// <summary>
    /// Check if exist some <see cref="TEntity"/> with defined criteria
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}