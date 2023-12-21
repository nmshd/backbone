using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsByIdentity;

public class FindRelationshipsByIdentityResponse
{
    public IEnumerable<Relationship> Relationships { get; set; }
}
