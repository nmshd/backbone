namespace Backbone.AdminApi.Tests.Integration.Models;
public class UpdateClientRequest
{
    public required string DefaultTier { get; set; }
    public int? MaxIdentities { get; set; }
}
