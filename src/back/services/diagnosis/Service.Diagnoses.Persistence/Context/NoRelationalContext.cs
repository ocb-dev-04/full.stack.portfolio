using MongoDB.Driver;
using Shared.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Service.Diagnoses.Persistence.Context;

public sealed class NoRelationalContext : IDisposable
{
    private readonly List<Func<CancellationToken, Task>> _commands;

    private int ChangeCount;
    private IMongoDatabase Database { get; init; }

    /// <summary>
    /// <see cref="NoRelationalContext"/> public constructor
    /// </summary>
    /// <param name="databaseOptions"></param>
    public NoRelationalContext(IOptions<NoRelationalDatabaseSettings> databaseOptions)
    {
        ArgumentNullException.ThrowIfNull(databaseOptions);

        _commands = new ();
        ChangeCount = 0;

        MongoClient mongoClient = new(databaseOptions.Value.ConnectionString);
        Database = mongoClient.GetDatabase(databaseOptions.Value.DatabaseName);
    }

    /// <summary>
    /// Save all change maked
    /// </summary>
    /// <returns></returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Task> tasks = GetTasks(cancellationToken);
        await Task.WhenAll(tasks);
        
        _commands.Clear();
    }

    private IEnumerable<Task> GetTasks(CancellationToken cancellationToken)
    {
        foreach (Func<CancellationToken, Task> command in _commands)
            yield return command.Invoke(cancellationToken);
    }

    /// <summary>
    /// Check if change or commands list has any
    /// </summary>
    /// <returns></returns>
    public bool HasChanges()
        => _commands.Any();

    /// <summary>
    /// Add commnad waiting to call the <see cref="SaveChanges"/> method
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public Task AddCommand(Func<CancellationToken, Task> func)
    {
        _commands.Add(func);
        ChangeCount = _commands.Count;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Get collection. Can add some filters and more
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IMongoCollection<T> GetCollection<T>()
        => Database.GetCollection<T>(typeof(T).Name.ToLower());

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
        => GC.SuppressFinalize(this);
}
