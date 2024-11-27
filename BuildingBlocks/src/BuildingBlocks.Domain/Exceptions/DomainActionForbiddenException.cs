using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.BuildingBlocks.Domain.Exceptions;

public class DomainActionForbiddenException : DomainException
{
    public DomainActionForbiddenException() : base(GenericDomainErrors.Forbidden())
    {
    }

    public DomainActionForbiddenException(Exception innerException) : base(GenericDomainErrors.Forbidden(), innerException)
    {
    }
}
