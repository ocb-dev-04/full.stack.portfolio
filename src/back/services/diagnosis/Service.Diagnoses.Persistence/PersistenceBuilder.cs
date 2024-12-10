using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Domain.Settings;

namespace Service.Diagnoses.Persistence;

public static class PersistenceBuilder
{
    private static readonly string _bsonDocCollectionKey = "name";

    public static void CheckCollectionExist(this WebApplication app)
    {
        NoRelationalDatabaseSettings? settings = app.Services.GetRequiredService<IOptions<NoRelationalDatabaseSettings>>().Value;
        using MongoClient client = new MongoClient(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

        HashSet<string> collection = database.ListCollections()
            .ToEnumerable()
            .Select(s => s[_bsonDocCollectionKey].AsString).ToHashSet();

        foreach (string item in settings.Collections)
            if(!collection.Contains(item))
                database.CreateCollection(item);
    }
}
