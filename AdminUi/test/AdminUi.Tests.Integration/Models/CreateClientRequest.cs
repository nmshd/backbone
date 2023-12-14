namespace Backbone.AdminUi.Tests.Integration.Models;

public class CreateClientRequest
{
    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string DefaultTier { get; set; }
    public int? MaxIdentities { get; set; }
}
