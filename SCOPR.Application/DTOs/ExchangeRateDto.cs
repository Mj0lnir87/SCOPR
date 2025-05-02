using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SCOPR.Application.DTOs;

/// <summary>
/// Represents the response from the ExchangeRate API.
/// </summary>
public class ExchangeRateDto
{
    public bool success { get; set; }
    public int timestamp { get; set; }
    public string @base { get; set; }
    public DateTime date { get; set; }
    public Rates rates { get; set; }
    public Error error { get; set; }

}

/// <summary>
/// Represents the rates of different currencies.
/// </summary>
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

/// <summary>
/// Represents an error response from the ExchangeRate API.
/// </summary>
public class Error
{
    public int code { get; set; }
    public string type { get; set; }
    public string info { get; set; }
}