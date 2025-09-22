namespace Backbone.AdminCli.Commands.Database.Model;

public class RelationshipTemplateExport
{
    public required string TemplateId { get; set; }
    public required string CreatedBy { get; set; }
    public required string? CreatedByClientId { get; set; }
    public required string? CreatedByClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? AllocatedAt { get; set; }
}
