using System.Globalization;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public record ExhaustionDate(DateTime Value) : IComparable<ExhaustionDate>
{
    public static readonly ExhaustionDate UNEXHAUSTED = new(DateTime.MinValue);

    public readonly DateTime Value = Value;

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    public static bool operator <=(ExhaustionDate left, ExhaustionDate right)
    {
        return left < right || left == right;
    }

    public static bool operator <(ExhaustionDate left, ExhaustionDate right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >=(ExhaustionDate left, ExhaustionDate right)
    {
        return left > right || left == right;
    }

    public static bool operator >(ExhaustionDate left, ExhaustionDate right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <(ExhaustionDate left, DateTime right)
    {
        return left.Value < right;
    }

    public static bool operator >(ExhaustionDate left, DateTime right)
    {
        return left.Value > right;
    }

    public int CompareTo(ExhaustionDate? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Value.CompareTo(other.Value);
    }
}
