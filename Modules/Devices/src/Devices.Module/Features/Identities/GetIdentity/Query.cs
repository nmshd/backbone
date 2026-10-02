using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetIdentityQuery")]
public class Query : IRequest<Response>
{
    public required string Address { get; init; }
}
