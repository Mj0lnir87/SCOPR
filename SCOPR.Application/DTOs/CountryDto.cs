using Newtonsoft.Json.Linq;

namespace SCOPR.Application.DTOs;

/// <summary>
/// Represents a country data transfer object.
/// </summary>
public class CountryDto
{
    public Name name { get; set; }
    public string cioc { get; set; }
    public Currencies currencies { get; set; }
    public Idd idd { get; set; }
    public List<string> capital { get; set; }
    public string flag { get; set; }
    public int population { get; set; }
}

/// <summary>
/// Represents a collection of currencies.
/// </summary>
public class Currencies
{
    [Newtonsoft.Json.JsonExtensionData]
    public Dictionary<string, JToken> Items { get; set; } = new Dictionary<string, JToken>();

    // Indexer for easy access to currencies by code
    public JToken? this[string currencyCode]
    {
        get => Items.TryGetValue(currencyCode, out var currency) ? currency : null;
    }
    
    public IEnumerable<string> AvailableCurrencies => Items.Keys;

    // Helper methods to get specific properties
    public string GetCurrencyCode()
    {
        // Get the first available currency code
        if (!AvailableCurrencies.Any())
            return "N/A";
        return AvailableCurrencies.First();
    }

    public string GetCurrencySymbol(string currencyCode)
    {
        return this[currencyCode]?["symbol"]?.ToString() ?? "N/A";
    }

    public string GetCurrencyName(string currencyCode)
    {
        return this[currencyCode]?["name"]?.ToString() ?? "N/A";
    }
}

/// <summary>
/// Represents the International Direct Dialing (IDD) information.
/// </summary>
public class Idd
{
    public string root { get; set; }
    public List<string> suffixes { get; set; }
}

/// <summary>
/// Represents the name of a country.
/// </summary>
public class Name
{
    public string common { get; set; }
    public string official { get; set; }
}