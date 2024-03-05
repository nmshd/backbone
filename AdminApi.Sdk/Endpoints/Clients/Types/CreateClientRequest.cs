namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types;

public class CreateClientRequest
{
    public string? ClientId { get; set; }
    public string? DisplayName { get; set; }
    public string? ClientSecret { get; set; }
    public required string DefaultTier { get; set; }
    public int? MaxIdentities { get; set; }
}
