using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SCOPR.Domain.Entities;

public class Country
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public IList<string> PhoneCodes { get; set; }
    public string Capital { get; set; }
    public int Population { get; set; }
    public Currency Currency { get; set; }
    public string Flag { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Currency
{
    public string Code { get; }
    public string Name { get; private set; }
    public string Symbol { get; private set; }

    public Currency(string code, string name, string symbol)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
    }
}