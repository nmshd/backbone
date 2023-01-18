using MediatR;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Commands.CreateRelationshipTerminationRequest;

public class CreateRelationshipTerminationRequestCommand : IRequest<RelationshipChangeMetadataDTO>
{
    public RelationshipId Id { get; set; }
}
