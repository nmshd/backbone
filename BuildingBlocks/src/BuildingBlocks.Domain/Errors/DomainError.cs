namespace Backbone.BuildingBlocks.Domain.Errors;

public class DomainError
{
    public DomainError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    protected bool Equals(DomainError other)
    {
        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DomainError)obj);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
