using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

[ApplyQuotasForMetrics("NumberOfRelationshipTemplates")]
public class CreateRelationshipTemplateCommand : IRequest<CreateRelationshipTemplateResponse>
{
    public DateTime? ExpiresAt { get; init; }
    public int? MaxNumberOfAllocations { get; init; }
    public required byte[] Content { get; init; }
    public string? ForIdentity { get; init; }
    public byte[]? Password { get; init; }
}
