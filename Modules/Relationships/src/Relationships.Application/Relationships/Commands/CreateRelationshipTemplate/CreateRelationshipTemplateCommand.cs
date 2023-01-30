using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateCommand : IMapTo<RelationshipTemplate>, IRequest<CreateRelationshipTemplateResponse>
{
    public DateTime? ExpiresAt { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public byte[] Content { get; set; }
}
