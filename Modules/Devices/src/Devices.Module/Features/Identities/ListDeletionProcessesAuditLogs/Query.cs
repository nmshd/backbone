using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;

public class ListDeletionProcessesAuditLogsQuery : IRequest<ListDeletionProcessesAuditLogsResponse>
{
    public required string IdentityAddress { get; init; }
}
