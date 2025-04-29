using MediatR;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using SCOPR.Domain.Enums;

namespace SCOPR.Application.Commands.GenerateReports;

public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, byte[]>
{
    private readonly IReportGenerator _reportGenerator;
    private readonly IExchangeRateRepository _exchangeRateRepository;
    private readonly ICountryRepository _countryRepository;

    public GenerateReportCommandHandler(
        IReportGenerator reportGenerator,
        IExchangeRateRepository exchangeRateRepository,
        ICountryRepository countryRepository)
    {
        _reportGenerator = reportGenerator;
        _exchangeRateRepository = exchangeRateRepository;
        _countryRepository = countryRepository;
    }

    public async Task<byte[]> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        List<Country> countries = await _countryRepository.GetAllByCodesAsync(request.CountryCodes);
        List<CountrySummary> countrySummaries = new List<CountrySummary>();

        foreach (var country in countries)
        {
            //calculate the average exchange rate for the country in the given period
            var exchangeRates = await _exchangeRateRepository.GetAverageRateInPeriodAsync(CurrencyCode.EUR.ToString(), country.Currency.Code ,request.StartDate, request.EndDate);

            CountrySummary summary = await _countryRepository.GetSummaryInPeriodAsync(country.Code, request.StartDate, request.EndDate);
            summary.AverageExchangeRate = exchangeRates;
            countrySummaries.Add(summary);
        }

        return await _reportGenerator.GenerateCountrySummaryReportAsync(countrySummaries);
    }
}