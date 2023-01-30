using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTerminationRequest;

public class CreateRelationshipTerminationRequestCommand : IRequest<RelationshipChangeMetadataDTO>
{
    public RelationshipId Id { get; set; }
}
