using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

namespace Backbone.AdminCli.Commands.Database.Model;

public class DatawalletModificationExport
{
    public required string DatawalletModificationId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string? CreatedByClientName { get; set; }
    public required string ObjectIdentifier { get; set; }
    public required string Collection { get; set; }
    public required DatawalletModificationType Type { get; set; }
    public required string? PayloadCategory { get; set; }
    public required long? PayloadSize { get; set; }
}
