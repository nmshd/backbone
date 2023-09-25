using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

[ApplyQuotasForMetrics("NumberOfRelationshipTemplates")]
public class CreateRelationshipTemplateCommand : IMapTo<RelationshipTemplate>, IRequest<CreateRelationshipTemplateResponse>
{
    public DateTime? ExpiresAt { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public byte[] Content { get; set; }
}
