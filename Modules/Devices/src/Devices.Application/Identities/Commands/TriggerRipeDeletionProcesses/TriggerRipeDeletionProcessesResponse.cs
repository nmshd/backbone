using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
public class TriggerRipeDeletionProcessesResponse
{
    public TriggerRipeDeletionProcessesResponse(Dictionary<IdentityAddress, UnitResult<DomainError>> results)
    {
        Results = results;
    }

    public Dictionary<IdentityAddress, UnitResult<DomainError>> Results { get; }
}
