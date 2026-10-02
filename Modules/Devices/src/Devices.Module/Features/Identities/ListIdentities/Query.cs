using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListIdentities;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListIdentitiesQuery")]
public class Query : IRequest<Response>
{
    public required List<string>? Addresses { get; init; }
    public required IdentityStatus? Status { get; init; }
}
