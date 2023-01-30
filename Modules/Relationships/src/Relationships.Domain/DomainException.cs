using Backbone.Modules.Relationships.Domain.Errors;

namespace Backbone.Modules.Relationships.Domain;

public class DomainException : Exception
{
    public DomainException(DomainError error) : base(error.Message)
    {
        Error = error;
    }

    public DomainError Error { get; }
}
