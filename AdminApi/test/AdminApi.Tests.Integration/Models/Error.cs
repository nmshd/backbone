namespace Backbone.AdminApi.Tests.Integration.Models;

public class Error
{
    public required string Id { get; set; }
    public required string Code { get; set; }
    public required string Message { get; set; }
    public required string Docs { get; set; }
    public required DateTime Time { get; set; }
}
