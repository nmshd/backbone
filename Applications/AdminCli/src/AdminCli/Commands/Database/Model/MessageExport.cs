namespace Backbone.AdminCli.Commands.Database.Model;

public class MessageExport
{
    public required string MessageId { get; set; }
    public required string CreatedBy { get; set; }
    public required string? CreatedByClientId { get; set; }
    public required string? CreatedByClientName { get; set; }
    public required string RelationshipId { get; set; }
    public required string Recipient { get; set; }
    public required string? RecipientClientId { get; set; }
    public required string? RecipientClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ReceivedAt { get; set; }
    public required long CipherSize { get; set; }
}
