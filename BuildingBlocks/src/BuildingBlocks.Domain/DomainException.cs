using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Enmeshed.BuildingBlocks.Domain;

public class DomainException : Exception
{
    public DomainException(DomainError error) : base(error.Message)
    {
        Code = error.Code;
    }

    public DomainException(DomainError error, Exception innerException) : base(error.Message,
        innerException)
    {
        Code = error.Code;
    }

    public string Code { get; set; }
}