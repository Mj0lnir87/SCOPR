namespace SCOPR.API.Requests
{
    public class FetchExchangeRatesRequest
    {
        public string BaseCurrency { get; set; }
        public List<string> TargetCurrencies { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public FetchExchangeRatesRequest(string baseCurrency, List<string> targetCurrencies, DateTime startDate, DateTime endDate)
        {
            BaseCurrency = baseCurrency;
            TargetCurrencies = targetCurrencies;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
