using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class Handler : IRequestHandler<DecomposeAndAnonymizeRelationshipsOfIdentityCommand>
{
    public static ILogger<Handler>? Logger = null!;

    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IRelationshipsRepository relationshipsRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _relationshipsRepository = relationshipsRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(DecomposeAndAnonymizeRelationshipsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var relationships = (await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)).ToList();

        Logger?.LogError("Decomposing {n} relationships for identity {IdentityAddress}", relationships.Count, request.IdentityAddress);

        foreach (var relationship in relationships)
            relationship.DecomposeDueToIdentityDeletion(request.IdentityAddress, _applicationOptions.DidDomainName);

        Logger?.LogError("Done decomposing relationships for identity {IdentityAddress}", request.IdentityAddress);

        var rel = relationships.FirstOrDefault();

        Logger?.LogError("Relationship: {rel}", JsonSerializer.Serialize(rel, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        await _relationshipsRepository.Update(relationships);
    }
}
