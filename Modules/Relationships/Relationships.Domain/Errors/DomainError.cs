namespace Relationships.Domain.Errors;

public class DomainError : IEquatable<DomainError>
{
    public DomainError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public bool Equals(DomainError? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((DomainError) obj);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public static bool operator ==(DomainError? left, DomainError? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DomainError? left, DomainError? right)
    {
        return !Equals(left, right);
    }
}
