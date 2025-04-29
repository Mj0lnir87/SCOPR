using MediatR;

namespace SCOPR.Application.Commands.GenerateReports;

public class GenerateReportCommand : IRequest<byte[]>
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public IList<string> CountryCodes { get; }

    public GenerateReportCommand(DateTime startDate, DateTime endDate, IList<string> countryCodes)
    {
        StartDate = startDate;
        EndDate = endDate;
        CountryCodes = countryCodes;
    }

}