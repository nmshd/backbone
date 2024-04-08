namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string recordName = "") : base(
        GenericApplicationErrors.NotFound(recordName))
    {
    }

    public NotFoundException(string recordName, Exception innerException) : base(
        GenericApplicationErrors.NotFound(recordName), innerException)
    {
    }

    public NotFoundException(Exception innerException) : base(
        GenericApplicationErrors.NotFound(), innerException)
    {
    }
}
