using MediatR;

namespace SCOPR.Application.Commands.FetchCountries;

public class FetchCountriesCommand : IRequest
{
    public IList<string> CountryCodes { get; }
    public FetchCountriesCommand(IList<string> countryCodes)
    {
        CountryCodes = countryCodes;
    }
}