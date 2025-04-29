namespace SCOPR.Domain.Entities;

public class ExchangeRate
{
    public string Id { get; set; }

    public string BaseCurrencyCode { get; set; }

    public string TargetCurrencyCode { get; set; }

    public decimal Rate { get; set; }

    public DateTime CreatedAt { get; set; }
}