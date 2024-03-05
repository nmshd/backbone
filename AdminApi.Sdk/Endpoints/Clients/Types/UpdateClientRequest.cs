namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types;

public class UpdateClientRequest
{
    public required string DefaultTier { get; set; }
    public int? MaxIdentities { get; set; }
}
