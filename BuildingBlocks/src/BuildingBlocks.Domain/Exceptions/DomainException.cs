using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.BuildingBlocks.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(DomainError error) : base(error.Message)
    {
        Error = error;
    }

    public DomainException(DomainError error, Exception innerException) : base(error.Message,
        innerException)
    {
        Error = error;
    }

    public string Code => Error.Code;
    public DomainError Error { get; set; }
}
