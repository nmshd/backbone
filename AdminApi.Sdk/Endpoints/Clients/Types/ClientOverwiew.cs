namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types;

public class ClientOverwiew
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required Tier DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
    public required int NumberOfIdentities { get; set; }
}

public class Tier
{
    public required string Id { get; set; }
    public required string Name { get; set; }
}
