using SCOPR.Domain.Enums;

namespace SCOPR.Domain.Entities;

public class Currency
{
    public string Code { get; }
    public string Name { get; private set; }
    public string Symbol { get; private set; }

    private static readonly string EUR = CurrencyCode.EUR.ToString();
    private static readonly string AUD = CurrencyCode.AUD.ToString();
    private static readonly string BRL = CurrencyCode.BRL.ToString();
    private static readonly string CNY = CurrencyCode.CNY.ToString();
    private static readonly string GBP = CurrencyCode.GBP.ToString();
    private static readonly string USD = CurrencyCode.USD.ToString();

    public Currency(string code, string name, string symbol)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
    }

    // Value equality
    public override bool Equals(object? obj)
    {
        if (obj is not Currency other)
            return false;

        return Code == other.Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    private static readonly Dictionary<string, Currency> _knownCurrencies = new()
    {
        [EUR] = new Currency(EUR, "Euro", "€"),
        [USD] = new Currency(USD, "US Dollar", "$"),
        [GBP] = new Currency(GBP, "British Pound", "£"),
        [AUD] = new Currency(AUD, "Australian Dollar", "A$"),
        [BRL] = new Currency(BRL, "Brazilian Real", "R$"),
        [CNY] = new Currency(CNY, "Chinese Yuan", "¥")
    };

    // Factory method
    public static Currency FromCode(string code)
    {
        if (_knownCurrencies.TryGetValue(code, out var currency))
            return currency;

        // Default value if not found
        return new Currency(code, $"{code} Currency", code);
    }
}