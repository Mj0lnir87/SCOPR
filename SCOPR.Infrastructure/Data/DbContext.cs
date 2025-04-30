using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SCOPR.Infrastructure.Data;

public class DbContext
{
    private readonly IMongoDatabase _db; 

    public DbContext(IConfiguration config)
    {
        string connectionString = config.GetConnectionString("DefaultConnection");
        string databaseName = config.GetSection("ConnectionStrings:DatabaseName").Value;

        ValidateConnectionsettings(connectionString, databaseName);

        var client = new MongoClient(connectionString);
        _db = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _db.GetCollection<T>(name);
    }

    private void ValidateConnectionsettings(string connectionString, string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(databaseName))
        {
            throw new ArgumentNullException(nameof(databaseName), "Database name cannot be null or empty.");
        }
        if (!connectionString.StartsWith("mongodb://"))
        {
            throw new ArgumentException("Invalid connection string format. It should start with 'mongodb://'.", nameof(connectionString));
        }
        if (connectionString.Contains(" "))
        {
            throw new ArgumentException("Connection string cannot contain spaces.", nameof(connectionString));
        }
        if (databaseName.Contains(" "))
        {
            throw new ArgumentException("Database name cannot contain spaces.", nameof(databaseName));
        }
    }
}