namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class OperationFailedException : ApplicationException
{
    public OperationFailedException(ApplicationError error) : base(error)
    {
    }

    public OperationFailedException(ApplicationError error, Exception innerException) : base(error, innerException)
    {
    }
}
