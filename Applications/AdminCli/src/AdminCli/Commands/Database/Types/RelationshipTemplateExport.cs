namespace Backbone.AdminCli.Commands.Database.Types;

public class RelationshipTemplateExport
{
    public required string TemplateId { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? AllocatedAt { get; set; }
}
