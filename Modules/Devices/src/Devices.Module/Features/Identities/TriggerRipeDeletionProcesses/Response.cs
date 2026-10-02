using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Module.Features.Identities.TriggerRipeDeletionProcesses;

public class Response
{
    public Response()
    {
        Results = new Dictionary<string, UnitResult<DomainError>>();
    }

    public Response(Dictionary<string, UnitResult<DomainError>> results)
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
