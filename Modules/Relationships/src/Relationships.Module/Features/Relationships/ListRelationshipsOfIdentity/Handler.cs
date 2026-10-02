using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationshipsOfIdentity;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var relationships = await _relationshipsRepository.ListWithoutContent(Relationship.HasParticipant(request.IdentityAddress), cancellationToken);

        return new Response(relationships);
    }
}
