using Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;

namespace Backbone.AdminCli.Commands.Database.Model;

public class RelationshipExport
{
    public required string RelationshipId { get; set; }
    public required string? TemplateId { get; set; }
    public required string From { get; set; }
    public required string? FromClientName { get; set; }
    public required string To { get; set; }
    public required string? ToClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required RelationshipStatus Status { get; set; }
    public required bool FromHasDecomposed { get; set; }
    public required bool ToHasDecomposed { get; set; }
    public required DateTime? TemplateCreatedAt { get; set; }
}
