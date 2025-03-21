using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetFeatureFlags;

public class Handler : IRequestHandler<GetFeatureFlagsQuery, GetFeatureFlagsResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<GetFeatureFlagsResponse> Handle(GetFeatureFlagsQuery request, CancellationToken cancellationToken)
    {
        // TODO : Check for existing relationship template allocation 
        var featureFlags = await _identityRepository.GetAllFeatureFlagsOfIdentity(request.IdentityAddress, cancellationToken);
        return new GetFeatureFlagsResponse(featureFlags);
    }
}
