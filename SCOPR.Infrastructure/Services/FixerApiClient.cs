using System.Net;
using Microsoft.Extensions.Configuration;
using SCOPR.Application.DTOs;
using SCOPR.Application.Interfaces;

namespace SCOPR.Infrastructure.Services;

public class FixerApiClient : IExchangeRateApiClient
{
    private static readonly string _section = "Fixer";
    private static readonly string _url = "Url";
    private static readonly string _apiKey = "ApiKey";

    private readonly string apiKey;
    private readonly HttpClient _httpClient;

    public FixerApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;

        var section = config.GetSection(_section);
        apiKey = section[_apiKey];

        // Set base address for the HttpClient
        _httpClient.BaseAddress = new Uri(section[_url]);
    }

    /// <summary>
    /// Get the latest exchange rates for a given base currency and a list of target currencies.
    /// </summary>
    /// <param name="baseCurrency"></param>
    /// <param name="targetCurrencies"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<IList<ExchangeRateDto>> GetLatestRatesAsync(string baseCurrency, List<string> targetCurrencies, DateTime startDate, DateTime endDate)
    {
        // Validate the input
        if (string.IsNullOrWhiteSpace(baseCurrency) || targetCurrencies == null || !targetCurrencies.Any())
        {
            throw new ArgumentNullException(nameof(baseCurrency));
        }

        IList<ExchangeRateDto> exchangeRates = new List<ExchangeRateDto>();

        // Convert the date range to a list
        var dates = new List<string>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            dates.Add(date.ToString("yyyy-MM-dd"));
        }

        // Call the API to get the latest exchange rates
        var targetCurrenciesString = string.Join(",", targetCurrencies);

        foreach (var date in dates)
        {
            var response = await _httpClient.GetAsync($"{date}?access_key={apiKey}&base={baseCurrency}&symbols={targetCurrenciesString}");
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new KeyNotFoundException(response.ReasonPhrase);
                    case HttpStatusCode.BadRequest:
                        throw new ArgumentException(response.ReasonPhrase);
                    case HttpStatusCode.InternalServerError:
                    default:
                        throw new Exception(response.ReasonPhrase);
                }
            }

            // Deserialize the response to ExchangeRateDto using Newtonsoft.Json
            var content = await response.Content.ReadAsStringAsync();
            var exchangeRateDto = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeRateDto>(content);
            if (exchangeRateDto == null)
            {
                // Handle the case where no exchange rate data is found
                throw new KeyNotFoundException($"Exchange rate data not found for base currency: {baseCurrency} and targets: {targetCurrenciesString}");
            }

            // Check if the response indicates success
            if (!exchangeRateDto.success)
            {
                throw new Exception($"Failed to fetch exchange rates: {exchangeRateDto.error.code}: {exchangeRateDto.error.type} | {exchangeRateDto.error.info}");
            }

            // Add the exchange rate to the list
            exchangeRates.Add(exchangeRateDto);

            // Added a timeout because the API throws an error stating "106: rate_limit_reached".
            // It's because we're on a free plan. While it's not ideal, it's a workaround.
            // This would never be done in production code!!!
            await Task.Delay(1000);
        }

        return exchangeRates;
    }
}