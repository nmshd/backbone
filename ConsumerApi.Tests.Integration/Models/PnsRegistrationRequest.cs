namespace Backbone.ConsumerApi.Tests.Integration.Models;
internal class PnsRegistrationRequest
{
    public required string Platform { get; set; }
    public required string Handle { get; set; }
    public required string AppId { get; set; }
    public string? Environment { get; set; }
}
