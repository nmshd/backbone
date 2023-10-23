using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

[ApplyQuotasForMetrics("NumberOfRelationshipTemplates")]
public class CreateRelationshipTemplateCommand : IMapTo<RelationshipTemplate>, IRequest<CreateRelationshipTemplateResponse>
{
    public DateTime? ExpiresAt { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public byte[] Content { get; set; }
}
