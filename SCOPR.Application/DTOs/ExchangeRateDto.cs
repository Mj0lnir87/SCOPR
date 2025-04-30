using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SCOPR.API.DTOs;

public class ExchangeRateDto
{
    /*public string BaseCurrencyCode { get; set; }
    public string TargetCurrencyCode { get; set; }
    public decimal Rate { get; set; }
    public DateTime Timestamp { get; set; }

    public ExchangeRateDto(string baseCurrencyCode, string targetCurrencyCode, decimal rate, DateTime timestamp)
    {
        BaseCurrencyCode = baseCurrencyCode;
        TargetCurrencyCode = targetCurrencyCode;
        Rate = rate;
        Timestamp = timestamp;
    }*/

    public bool success { get; set; }
    public int timestamp { get; set; }
    public string @base { get; set; }
    public string date { get; set; }
    public Rates rates { get; set; }
    public Error error { get; set; }

}
public class Rates
{
    [Newtonsoft.Json.JsonExtensionData]
    public Dictionary<string, JToken> Items { get; set; } = new Dictionary<string, JToken>();

    // Indexer for easy access to currencies by code
    public JToken? this[string currencyCode]
    {
        get => Items.TryGetValue(currencyCode, out var currency) ? currency : null;
    }
}

public class Error
{
    public int code { get; set; }
    public string type { get; set; }
    public string info { get; set; }
}