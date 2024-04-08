namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public class ActionForbiddenException : ApplicationException
{
    public ActionForbiddenException() : base(GenericApplicationErrors.Forbidden())
    {
    }

    public ActionForbiddenException(Exception innerException) : base(GenericApplicationErrors.Forbidden(),
        innerException)
    {
    }
}
