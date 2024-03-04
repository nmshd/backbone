namespace Backbone.Modules.Synchronization.Domain;

[Serializable]
public abstract class SimpleValueObject<T> : ValueObject
{
    protected SimpleValueObject(T value)
    {
        Value = value;
    }

    public T Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string? ToString()
    {
        return Value == null ? "<null>" : Value.ToString();
    }

    public static implicit operator T(SimpleValueObject<T>? valueObject)
    {
#pragma warning disable 8603
        return valueObject == null ? default : valueObject.Value;
#pragma warning restore 8603
    }
}
