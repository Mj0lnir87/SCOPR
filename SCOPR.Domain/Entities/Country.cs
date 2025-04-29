namespace SCOPR.Domain.Entities;

public class Country
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string PhoneCode { get; set; }
    public string Capital { get; set; }
    public int Population { get; set; }
    public Currency Currency { get; set; }
    public string FlagUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}