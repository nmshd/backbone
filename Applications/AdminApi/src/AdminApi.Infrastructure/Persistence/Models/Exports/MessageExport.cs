namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Exports;

public class MessageExport
{
    public required string MessageId { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required string RelationshipId { get; set; }
    public required string Recipient { get; set; }
    public string? RecipientClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ReceivedAt { get; set; }
    public required long CipherSize { get; set; }
}
