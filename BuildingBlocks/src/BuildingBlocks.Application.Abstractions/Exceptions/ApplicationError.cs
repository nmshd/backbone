namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class ApplicationError
{
    public ApplicationError(string code, string message, dynamic? additionalData = null)
    {
        Code = code;
        Message = message;
        AdditionalData = additionalData;
    }

    public string Code { get; }
    public string Message { get; }
    public dynamic? AdditionalData { get; }
}
