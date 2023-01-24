namespace Relationships.Common;

public class Range<T>
{
    public Range() { }

    public Range(T from, T to)
    {
        From = from;
        To = to;
    }

    public T From { get; set; }
    public T To { get; set; }

    public bool HasFrom()
    {
        return !From.Equals(default(T));
    }

    public bool HasTo()
    {
        return !To.Equals(default(T));
    }
}

public class OptionalDateRange : Range<DateTime?>
{
    public OptionalDateRange() { }

    public OptionalDateRange(DateTime? from, DateTime? to) : base(from, to)
    {
        From = from;
        To = to;
    }
}
