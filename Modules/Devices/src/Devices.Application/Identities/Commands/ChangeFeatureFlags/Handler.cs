using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ChangeFeatureFlags;

public class Handler : IRequestHandler<ChangeFeatureFlagsCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task Handle(ChangeFeatureFlagsCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_activeIdentity, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        identity.ChangeFeatureFlags(request.ToDictionary(kv => FeatureFlagName.Parse(kv.Key), kv => kv.Value));
        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
