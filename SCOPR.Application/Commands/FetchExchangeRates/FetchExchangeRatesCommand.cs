using MediatR;

namespace SCOPR.Application.Commands.FetchExchangeRates;

public class FetchExchangeRatesCommand : IRequest
{
    public string BaseCurrency { get; }
    public List<string> TargetCurrencies { get; }

    public FetchExchangeRatesCommand(string baseCurrency, List<string> targetCurrencies)
    {
        BaseCurrency = baseCurrency;
        TargetCurrencies = targetCurrencies;
    }
}