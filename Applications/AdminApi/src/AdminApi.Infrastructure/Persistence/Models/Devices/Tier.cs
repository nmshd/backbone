// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Tier
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required bool CanBeManuallyAssigned { get; init; }
    public required bool CanBeUsedAsDefaultForClient { get; init; }
}
