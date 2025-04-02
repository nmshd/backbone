using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetFeatureFlags;

public class GetFeatureFlagsQuery : IRequest<GetFeatureFlagsResponse>
{
    public required string IdentityAddress { get; set; }
}
