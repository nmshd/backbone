﻿namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types;

public class ClientInfo
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required string DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int NumberOfIdentities { get; set; }
    public int? MaxIdentities { get; set; }
}
