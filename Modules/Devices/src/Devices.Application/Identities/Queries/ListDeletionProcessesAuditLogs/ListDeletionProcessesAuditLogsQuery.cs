using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAuditLogs;

public class ListDeletionProcessesAuditLogsQuery : IRequest<ListDeletionProcessesAuditLogsResponse>
{
    public required string IdentityAddress { get; init; }
}
