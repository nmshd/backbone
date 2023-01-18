using Relationships.Domain.Errors;

namespace Relationships.Domain;

public class DomainException : Exception
{
    public DomainException(DomainError error) : base(error.Message)
    {
        Error = error;
    }

    public DomainError Error { get; }
}
