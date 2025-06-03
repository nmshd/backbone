// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

public class Datawallet
{
    public required string Id { get; init; }
    public required string Owner { get; init; }
    public required ushort Version { get; init; }
}
