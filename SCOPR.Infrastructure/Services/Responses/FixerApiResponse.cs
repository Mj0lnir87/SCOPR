namespace SCOPR.Infrastructure.Services.Responses;

internal class FixerApiResponse
{
    public bool Success { get; set; }
    public long Timestamp { get; set; }
    public string Base { get; set; }
    public string Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
    public ErrorInfo Error { get; set; }

    public class ErrorInfo
    {
        public int Code { get; set; }
        public string Info { get; set; }
    }
}