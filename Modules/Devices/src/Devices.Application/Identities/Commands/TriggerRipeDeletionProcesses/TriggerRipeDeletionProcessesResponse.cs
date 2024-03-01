using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
public class TriggerRipeDeletionProcessesResponse : CollectionResponseBase<string>
{
    public TriggerRipeDeletionProcessesResponse(IEnumerable<string> deletedIdentityAddresses) : base(deletedIdentityAddresses)
    {
    }
}
