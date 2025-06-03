using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAuditLogs;

public class ListDeletionProcessesAuditLogsQuery : IRequest<ListDeletionProcessesAuditLogsResponse>
{
    public ListDeletionProcessesAuditLogsQuery(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
