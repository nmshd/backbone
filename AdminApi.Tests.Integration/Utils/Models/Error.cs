namespace AdminApi.Tests.Integration.Utils.Models;

public class Error
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Docs { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
