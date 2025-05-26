// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class OpenIddictApplication
{
    public required string Id { get; init; }
    public required string ClientId { get; init; }
    public required string DisplayName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string DefaultTier { get; init; }
    public required int? MaxIdentities { get; init; }
}
