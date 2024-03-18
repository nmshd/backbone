namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;

public class UpdateClientRequest
{
    public required string DefaultTier { get; set; }
    public int? MaxIdentities { get; set; }
}
