using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListDeletionProcessesAuditLogsResponse")]
public class Response : CollectionResponseBase<IdentityDeletionProcessAuditLogEntryDTO>
{
    public Response(IEnumerable<IdentityDeletionProcessAuditLogEntry> processes)
        : base(processes.Select(p => new IdentityDeletionProcessAuditLogEntryDTO(p)))
    {
    }
}
