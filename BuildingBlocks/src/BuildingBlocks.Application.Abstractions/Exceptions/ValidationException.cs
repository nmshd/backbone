namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class ValidationException : ApplicationException
{
    public ValidationException(ApplicationError error) : base(error)
    {
    }

    public ValidationException(ApplicationError error, Exception innerException) : base(error, innerException)
    {
    }
}
