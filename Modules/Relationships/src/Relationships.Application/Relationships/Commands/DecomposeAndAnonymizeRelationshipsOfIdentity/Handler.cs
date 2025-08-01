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
        var relationships = (await _relationshipsRepository.ListWithoutContent(Relationship.HasParticipant(request.IdentityAddress), cancellationToken, track: true)).ToList();

        foreach (var relationship in relationships)
            relationship.DecomposeDueToIdentityDeletion(request.IdentityAddress, _applicationConfiguration.DidDomainName);

        await _relationshipsRepository.Update(relationships);
    }
}
