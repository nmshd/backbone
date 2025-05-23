namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

public class Datawallet
{
    public string Id { get; set; } = null!;
    public string Owner { get; set; } = null!;
    public ushort Version { get; set; }
}
