namespace SCOPR.API.DTOs;

public class ExchangeRateDto
{
    public string BaseCurrencyCode { get; set; }
    public string TargetCurrencyCode { get; set; }
    public decimal Rate { get; set; }
    public DateTime Timestamp { get; set; }

    public ExchangeRateDto(string baseCurrencyCode, string targetCurrencyCode, decimal rate, DateTime timestamp)
    {
        BaseCurrencyCode = baseCurrencyCode;
        TargetCurrencyCode = targetCurrencyCode;
        Rate = rate;
        Timestamp = timestamp;
    }

}