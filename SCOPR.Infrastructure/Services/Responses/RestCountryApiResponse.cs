namespace SCOPR.Infrastructure.Services.Responses;

internal class RestCountryApiResponse
{
    public string Cca3 { get; set; }
    public NameInfo Name { get; set; }
    public IddInfo Idd { get; set; }
    public List<string> Capital { get; set; }
    public int Population { get; set; }
    public Dictionary<string, CurrencyInfo> Currencies { get; set; }
    public FlagInfo Flags { get; set; }

    public class NameInfo
    {
        public string Common { get; set; }
    }

    public class IddInfo
    {
        public string Root { get; set; }
        public List<string> Suffixes { get; set; }
    }

    public class CurrencyInfo
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
    }

    public class FlagInfo
    {
        public string Png { get; set; }
    }
}