using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("IsIdentityOfUserDeletedQuery")]
public class Query : IRequest<Response>
{
    public required string Username { get; init; }
}
