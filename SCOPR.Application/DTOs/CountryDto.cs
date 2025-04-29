namespace SCOPR.API.DTOs;

public class CountryDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string PhoneCode { get; set; }
    public string Capital { get; set; }
    public int Population { get; set; }
    public CurrencyDto Currency { get; set; }
    public string FlagUrl { get; set; }
}