namespace Backbone.BuildingBlocks.Infrastructure.Exceptions;

public class InfrastructureError
{
    public InfrastructureError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }
}
