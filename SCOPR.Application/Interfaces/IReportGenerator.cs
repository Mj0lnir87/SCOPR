using SCOPR.Domain.Entities;

namespace SCOPR.Application.Interfaces;

public interface IReportGenerator
{
    Task<byte[]> GenerateCountrySummaryReportAsync(List<CountrySummary> summaries);
}