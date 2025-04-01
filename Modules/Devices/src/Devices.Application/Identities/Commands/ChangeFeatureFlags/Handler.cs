using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ChangeFeatureFlags;

public class Handler : IRequestHandler<ChangeFeatureFlagsCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IdentityAddress _activeIdentity;
    private readonly ApplicationConfiguration _configuration;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IOptions<ApplicationConfiguration> configuration)
    {
        _identitiesRepository = identitiesRepository;
        _activeIdentity = userContext.GetAddress();
        _configuration = configuration.Value;
    }

    public async Task Handle(ChangeFeatureFlagsCommand request, CancellationToken cancellationToken)
    {

        var identity = await _identitiesRepository.FindByAddress(_activeIdentity, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        EnsureAllAdditionalFeatureFlagsCanBeCreated(request, identity);

        identity.ChangeFeatureFlags(request.ToDictionary(kv => FeatureFlagName.Parse(kv.Key), kv => kv.Value));
        await _identitiesRepository.Update(identity, cancellationToken);
    }

    private void EnsureAllAdditionalFeatureFlagsCanBeCreated(ChangeFeatureFlagsCommand request, Identity identity)
    {
        var combinedFeatureFlagNames = identity.FeatureFlags.Names.Select(n => n.Value).Concat(request.Keys).ToHashSet();
        if (combinedFeatureFlagNames.Count > _configuration.MaxNumberOfFeatureFlagsPerIdentity)
            throw new ApplicationException(ApplicationErrors.Devices.MaxNumberOfFeatureFlagsExceeded(_configuration.MaxNumberOfFeatureFlagsPerIdentity));
    }
}
