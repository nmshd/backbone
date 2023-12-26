using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsByIdentityResponse
{
    public IEnumerable<Relationship> Relationships { get; set; }
}
