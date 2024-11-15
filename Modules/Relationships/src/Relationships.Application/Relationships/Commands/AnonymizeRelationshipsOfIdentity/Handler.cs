using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AnonymizeRelationshipsOfIdentity;

public class Handler : IRequestHandler<AnonymizeRelationshipsOfIdentityCommand>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly ApplicationOptions _options;

    public Handler(IRelationshipsRepository relationshipsRepository, IOptions<ApplicationOptions> options)
    {
        _relationshipsRepository = relationshipsRepository;
        _options = options.Value;
    }

    public async Task Handle(AnonymizeRelationshipsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var relationships = (await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)).ToList();

        foreach (var relationship in relationships) relationship.AnonymizeParticipant(request.IdentityAddress, _options.DidDomainName);

        await _relationshipsRepository.Update(relationships);
    }
}
