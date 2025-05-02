using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SCOPR.Domain.Entities;

public class ExchangeRate
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string BaseCurrencyCode { get; set; }
    public string TargetCurrencyCode { get; set; }
    public DateTime Date { get; set; }
    public decimal Rate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}