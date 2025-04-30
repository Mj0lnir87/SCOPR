using System.Net;
using Microsoft.Extensions.Configuration;
using SCOPR.API.DTOs;
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

        //Set base address for the HttpClient
        _httpClient.BaseAddress = new Uri(section[_url]);
    }

    public async Task<ExchangeRateDto> GetLatestRatesAsync(string baseCurrency, List<string> targetCurrencies)
    {
        // Validate the input
        if (string.IsNullOrWhiteSpace(baseCurrency) || targetCurrencies == null || !targetCurrencies.Any())
        {
            throw new ArgumentNullException(nameof(baseCurrency));
        }

        // Call the API to get the latest exchange rates
        var targetCurrenciesString = string.Join(",", targetCurrencies);
        var response = await _httpClient.GetAsync($"latest?access_key={apiKey}&base={baseCurrency}&symbols={targetCurrenciesString}");
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

        // Return the exchange rate data
        return exchangeRateDto;
    }
}