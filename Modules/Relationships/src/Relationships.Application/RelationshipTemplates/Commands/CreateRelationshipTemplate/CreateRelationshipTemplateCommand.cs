using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

[ApplyQuotasForMetrics("NumberOfRelationshipTemplates")]
public class CreateRelationshipTemplateCommand : IRequest<CreateRelationshipTemplateResponse>
{
    public DateTime? ExpiresAt { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public required byte[] Content { get; set; }
    public string? ForIdentity { get; set; }
}
