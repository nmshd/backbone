using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;

public class CancelStaleIdentityDeletionProcessesResponse : CollectionResponseBase<string>
{
    public CancelStaleIdentityDeletionProcessesResponse(IEnumerable<IdentityDeletionProcessId> items) : base(items.Select(i => i.Value))
    {
    }
}
