using Services.Doctors.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using Services.Doctors.Domain.Entities;
using Services.Doctors.Domain.StrongIds;
using Services.Doctors.Domain.Abstractions;
using Services.Doctors.Persistence.Context;
using Shared.Common.Helper.ErrorsHandler;
using Value.Objects.Helper.Values.Primitives;
using Shared.Global.Sources.Extensions;

namespace Services.Doctors.Persistence.Repositories;

internal sealed class DoctorRepository
    : IDoctorRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<Doctor> _table;
    private readonly static int _pageSize = 10;

    public DoctorRepository(AppDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        _dbContext = dbContext;
        _table = dbContext.Set<Doctor>();
    }

    /// <inheritdoc/>
    public async Task<Result<Doctor>> ByIdAsync(DoctorId id, CancellationToken cancellationToken = default)
    {
        Doctor? found = await _table.FindAsync(id, cancellationToken);
        if (found is null)
            return Result.Failure<Doctor>(DoctorErrors.NotFound);

        return found;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Doctor>> ByNameAsync(StringObject name, int pageNumber, CancellationToken cancellationToken = default)
        => await _table.AsNoTracking()
                    .Where(w => w.NormalizedName.Contains(name.Value.NormalizeToFTS()))
                    .OrderBy(w => w.NormalizedName)
                    .Skip((pageNumber - 1) * _pageSize).Take(_pageSize)
                    .ToArrayAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task CreateAsync(Doctor model, CancellationToken cancellationToken)
        => await _table.AddAsync(model, cancellationToken);

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(DoctorId id, CancellationToken cancellationToken = default)
    {
        Doctor? found = await _table.FindAsync(id, cancellationToken);
        if (found is null)
            return Result.Failure(DoctorErrors.NotFound);

        _table.Remove(found);

        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_dbContext.ChangeTracker.HasChanges())
            await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
        => _dbContext.Dispose();
}
