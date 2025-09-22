using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;

public class TriggerRipeDeletionProcessesResponse
{
    public TriggerRipeDeletionProcessesResponse()
    {
        Results = new Dictionary<string, UnitResult<DomainError>>();
    }

    public TriggerRipeDeletionProcessesResponse(Dictionary<string, UnitResult<DomainError>> results)
    {
        Results = results;
    }

    public Dictionary<string, UnitResult<DomainError>> Results { get; }

    public void AddSuccess(string address)
    {
        Results.Add(address, UnitResult.Success<DomainError>());
    }

    public void AddError(string address, DomainError error)
    {
        Results.Add(address, UnitResult.Failure(error));
    }
}
