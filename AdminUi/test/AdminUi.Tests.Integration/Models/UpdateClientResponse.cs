﻿namespace Backbone.AdminUi.Tests.Integration.Models;
public class UpdateClientResponse
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required string DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
}
