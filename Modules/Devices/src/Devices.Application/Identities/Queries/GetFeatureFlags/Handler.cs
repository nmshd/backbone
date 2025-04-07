using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetFeatureFlags;

public class Handler : IRequestHandler<GetFeatureFlagsQuery, GetFeatureFlagsResponse>
{
    private readonly IIdentitiesRepository _identityRepository;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IIdentitiesRepository identityRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext,
        IRelationshipsRepository relationshipsRepository)
    {
        _identityRepository = identityRepository;
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _relationshipsRepository = relationshipsRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<GetFeatureFlagsResponse> Handle(GetFeatureFlagsQuery request, CancellationToken cancellationToken)
    {
        if (!await HasPermission(request.IdentityAddress, cancellationToken))
            throw new NotFoundException(nameof(Identity));

        var featureFlags = await _identityRepository.GetAllFeatureFlagsOfIdentity(request.IdentityAddress, cancellationToken);
        return new GetFeatureFlagsResponse(featureFlags);
    }

    private async Task<bool> HasPermission(IdentityAddress address, CancellationToken cancellationToken)
    {
        if (_activeIdentity == address)
            return true;

        if (await _relationshipsRepository.RelationshipExistsBetween(address, _activeIdentity))
            return true;

        return await _relationshipTemplatesRepository.AllocationExists(
            RelationshipTemplateAllocation.IsAllocatedBy(_activeIdentity)
                .And(RelationshipTemplateAllocation.BelongsToTemplateOf(address)),
            cancellationToken);
    }
}
