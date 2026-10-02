using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetOwnIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetOwnIdentityQuery")]
public class Query : IRequest<Response>;
