namespace Enmeshed.BuildingBlocks.Domain.Errors;

public class DomainError
{
    public DomainError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }
}