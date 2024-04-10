﻿namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;

public class CreateClientResponse
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required string ClientSecret { get; set; }
    public required string DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
}
