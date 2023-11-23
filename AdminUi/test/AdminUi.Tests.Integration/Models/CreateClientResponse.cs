namespace Backbone.AdminUi.Tests.Integration.Models;
public class CreateClientResponse
{
    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
}
