using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;

public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
