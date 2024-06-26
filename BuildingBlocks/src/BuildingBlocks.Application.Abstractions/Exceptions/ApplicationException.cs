namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class ApplicationException : Exception
{
    public ApplicationException(ApplicationError error) : base(error.Message)
    {
        Code = error.Code;
        AdditionalData = error.AdditionalData;
    }

    public ApplicationException(ApplicationError error, Exception innerException) : base(error.Message,
        innerException)
    {
        Code = error.Code;
        AdditionalData = error.AdditionalData;
    }

    public string Code { get; set; }
    public dynamic? AdditionalData { get; }
}
