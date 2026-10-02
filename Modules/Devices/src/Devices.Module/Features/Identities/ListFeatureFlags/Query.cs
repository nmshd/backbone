using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListFeatureFlagsQuery")]
public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
