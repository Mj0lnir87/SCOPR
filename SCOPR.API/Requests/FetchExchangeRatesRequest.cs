namespace SCOPR.API.Requests
{
    public class FetchExchangeRatesRequest
    {
        public string BaseCurrency { get; set; }
        public List<string> TargetCurrencies { get; set; }

        public FetchExchangeRatesRequest(string baseCurrency, List<string> targetCurrencies)
        {
            BaseCurrency = baseCurrency;
            TargetCurrencies = targetCurrencies;
        }
    }
}
