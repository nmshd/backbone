namespace Backbone.AdminApi.Tests.Integration.Models;
public class ChangeClientSecretResponse
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required string ClientSecret { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
}
