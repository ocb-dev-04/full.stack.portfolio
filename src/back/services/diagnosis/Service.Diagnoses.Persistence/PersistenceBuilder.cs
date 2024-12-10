using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Diagnoses.Persistence.Context;
using Shared.Domain.Settings;

namespace Service.Diagnoses.Persistence;

public static class PersistenceBuilder
{
    public static void CheckCollectionExist(this WebApplication app)
    {
        NoRelationalDatabaseSettings? settings = app.Services.GetRequiredService<IOptions<NoRelationalDatabaseSettings>>().Value;
        NoRelationalContext noRelationalContext = app.Services.GetRequiredService<NoRelationalContext>();
        
        using MongoClient client = new MongoClient(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

        string collectionName = settings.DatabaseName;
        BsonDocument filter = new BsonDocument("name", collectionName);
        IAsyncCursor<BsonDocument> collections = noRelationalContext.Database.ListCollections(new ListCollectionsOptions { Filter = filter });
        if (!collections.Any())
            database.CreateCollection(collectionName);
    }
}
