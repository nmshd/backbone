using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListFeatureFlags;

public class ListFeatureFlagsQuery : IRequest<ListFeatureFlagsResponse>
{
    public required string IdentityAddress { get; init; }
}
