namespace Backbone.Modules.Synchronization.Domain;

[Serializable]
public abstract class ValueObject : IComparable, IComparable<ValueObject>
{
    private int? _cachedHashCode;

    public virtual int CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        var thisType = GetUnproxiedType(this);
        var otherType = GetUnproxiedType(obj);

        if (thisType != otherType)
            return string.Compare(thisType.ToString(), otherType.ToString(), StringComparison.Ordinal);

        var other = (ValueObject)obj;

        var components = GetEqualityComponents().ToArray();
        var otherComponents = other.GetEqualityComponents().ToArray();

        return components.Select((t, i) => CompareComponents(t, otherComponents[i])).FirstOrDefault(comparison => comparison != 0);
    }

    public virtual int CompareTo(ValueObject? other)
    {
        return CompareTo(other as object);
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (GetUnproxiedType(this) != GetUnproxiedType(obj))
            return false;

        var valueObject = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        _cachedHashCode ??= GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    var hashCode = obj == null ? 0 : obj.GetHashCode();
                    return current * 23 + hashCode;
                }
            });

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return _cachedHashCode.Value;
    }

    private int CompareComponents(object? object1, object? object2)
    {
        if (object1 is null && object2 is null)
            return 0;

        if (object1 is null)
            return -1;

        if (object2 is null)
            return 1;

        if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
            return comparable1.CompareTo(comparable2);

        return object1.Equals(object2) ? 0 : -1;
    }

    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b)
    {
        return !(a == b);
    }

    internal static Type GetUnproxiedType(object obj)
    {
        const string efCoreProxyPrefix = "Castle.Proxies.";
        const string nHibernateProxyPostfix = "Proxy";

        var type = obj.GetType();
        var typeString = type.ToString();

        if (!typeString.Contains(efCoreProxyPrefix) && !typeString.EndsWith(nHibernateProxyPostfix))
            return type;

        return type.BaseType ?? type;
    }
}
