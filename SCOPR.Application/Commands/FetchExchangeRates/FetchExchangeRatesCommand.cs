using MediatR;

namespace SCOPR.Application.Commands.FetchExchangeRates;

public class FetchExchangeRatesCommand : IRequest<Unit>
{
    public string BaseCurrency { get; }
    public List<string> TargetCurrencies { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public FetchExchangeRatesCommand(string baseCurrency, List<string> targetCurrencies, DateTime startDate, DateTime endDate)
    {
        BaseCurrency = baseCurrency;
        TargetCurrencies = targetCurrencies;
        StartDate = startDate;
        EndDate = endDate;
    }
}