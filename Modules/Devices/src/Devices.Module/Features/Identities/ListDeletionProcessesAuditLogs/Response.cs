using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;

public class ListDeletionProcessesAuditLogsResponse : CollectionResponseBase<IdentityDeletionProcessAuditLogEntryDTO>
{
    public ListDeletionProcessesAuditLogsResponse(IEnumerable<IdentityDeletionProcessAuditLogEntry> processes)
        : base(processes.Select(p => new IdentityDeletionProcessAuditLogEntryDTO(p)))
    {
    }
}
