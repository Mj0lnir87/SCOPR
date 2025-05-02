namespace SCOPR.API.Requests
{
    public class FetchCountriesRequest
    {
        public IList<string> CountryCodes { get; set; }

        public FetchCountriesRequest(IList<string> countryCodes)
        {
            CountryCodes = countryCodes;
        }
    }
}