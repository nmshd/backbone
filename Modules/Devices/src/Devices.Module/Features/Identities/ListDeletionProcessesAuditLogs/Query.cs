using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListDeletionProcessesAuditLogsQuery")]
public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
