using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateRelationshipTemplateCommand")]
[ApplyQuotasForMetrics("NumberOfRelationshipTemplates")]
public class Command : IRequest<Response>
{
    public DateTime? ExpiresAt { get; init; }
    public int? MaxNumberOfAllocations { get; init; }
    public required byte[] Content { get; init; }
    public string? ForIdentity { get; init; }
    public byte[]? Password { get; init; }
}
