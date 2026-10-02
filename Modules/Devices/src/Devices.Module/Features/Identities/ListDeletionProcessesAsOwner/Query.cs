using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsOwner;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListDeletionProcessesAsOwnerQuery")]
public class Query : IRequest<Response>;
