using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class Handler : IRequestHandler<DecomposeAndAnonymizeRelationshipsOfIdentityCommand>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly ILogger<Handler> _logger;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IRelationshipsRepository relationshipsRepository, IOptions<ApplicationOptions> applicationOptions, ILogger<Handler> logger)
    {
        _relationshipsRepository = relationshipsRepository;
        _logger = logger;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(DecomposeAndAnonymizeRelationshipsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var relationships = (await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)).ToList();

        _logger.LogError("Decomposing {n} relationships for identity {IdentityAddress}", relationships.Count, request.IdentityAddress);

        foreach (var relationship in relationships)
            relationship.DecomposeDueToIdentityDeletion(request.IdentityAddress, _applicationOptions.DidDomainName);

        _logger.LogError("Done decomposing relationships for identity {IdentityAddress}", request.IdentityAddress);

        await _relationshipsRepository.Update(relationships);
    }
}
