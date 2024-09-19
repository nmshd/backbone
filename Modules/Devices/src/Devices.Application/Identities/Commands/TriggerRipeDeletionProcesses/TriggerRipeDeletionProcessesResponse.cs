using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
public class TriggerRipeDeletionProcessesResponse
{
    public TriggerRipeDeletionProcessesResponse()
    {
        Results = new Dictionary<IdentityAddress, UnitResult<DomainError>>();
    }

    public TriggerRipeDeletionProcessesResponse(Dictionary<IdentityAddress, UnitResult<DomainError>> results)
    {
        Results = results;
    }

    public Dictionary<IdentityAddress, UnitResult<DomainError>> Results { get; }

    public void AddSuccess(IdentityAddress address)
    {
        Results.Add(address, UnitResult.Success<DomainError>());
    }

    public void AddError(IdentityAddress address, DomainError error)
    {
        Results.Add(address, UnitResult.Failure(error));
    }
}
