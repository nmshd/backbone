using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
public class TriggerRipeDeletionProcessesResponse
{
    public Dictionary<IdentityAddress, UnitResult<DomainError>> DeletedIdentityAddresses { get; }

    public TriggerRipeDeletionProcessesResponse(Dictionary<IdentityAddress, UnitResult<DomainError>> deletedIdentityAddresses)
    {
        DeletedIdentityAddresses = deletedIdentityAddresses;
    }
}
