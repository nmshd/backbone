using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class Handler : IRequestHandler<DecomposeAndAnonymizeRelationshipsOfIdentityCommand>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public Handler(IRelationshipsRepository relationshipsRepository, IOptions<ApplicationConfiguration> applicationOptions)
    {
        _relationshipsRepository = relationshipsRepository;
        _applicationConfiguration = applicationOptions.Value;
    }

    public async Task Handle(DecomposeAndAnonymizeRelationshipsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        /*
         * We have to ignore the cache (and force a reload from db) here, because the data can get changed by the deletion of the relationship template,
         * therefore causing an error when trying to save the cached data to the db
         */
        var relationships = (await _relationshipsRepository.ListWithoutContent(Relationship.HasParticipant(request.IdentityAddress), cancellationToken, track: true, ignoreCache: true)).ToList();

        foreach (var relationship in relationships)
            relationship.DecomposeDueToIdentityDeletion(request.IdentityAddress, _applicationConfiguration.DidDomainName);

        await _relationshipsRepository.Update(relationships);
    }
}
