using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationshipsOfIdentity;

public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
