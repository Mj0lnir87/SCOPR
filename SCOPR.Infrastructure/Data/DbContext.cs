using MongoDB.Driver;

namespace SCOPR.Infrastructure.Data;

public class DbContext
{
    private readonly IMongoDatabase _db;

    public DbContext(string connectionString, string databaseName)
    {
        //todo: get settings from appsettings.json
        var client = new MongoClient(connectionString);
        _db = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _db.GetCollection<T>(name);
    }
}