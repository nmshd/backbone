using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;

public class GetDeletionProcessesAuditLogsQuery : IRequest<GetDeletionProcessesAuditLogsResponse>
{
    public GetDeletionProcessesAuditLogsQuery(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
