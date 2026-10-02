using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListAddressesOfIdentitiesWithDeletionProcessInStatusDeleting;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingQuery")]
public class Query : IRequest<Response>;
