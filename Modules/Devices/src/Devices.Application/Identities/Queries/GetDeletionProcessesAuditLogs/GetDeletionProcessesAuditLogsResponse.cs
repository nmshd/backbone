using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;

public class GetDeletionProcessesAuditLogsResponse : CollectionResponseBase<IdentityDeletionProcessAuditLogEntryDTO>
{
    public GetDeletionProcessesAuditLogsResponse(IEnumerable<IdentityDeletionProcessAuditLogEntry> processes)
        : base(processes.Select(p => new IdentityDeletionProcessAuditLogEntryDTO(p)))
    {
    }
}
