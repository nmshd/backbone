using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;

public class ListFeatureFlagsQuery : IRequest<ListFeatureFlagsResponse>
{
    public required string IdentityAddress { get; init; }
}
