using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;

public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
