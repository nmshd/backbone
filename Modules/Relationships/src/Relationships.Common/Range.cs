namespace Backbone.Modules.Relationships.Common;

public class Range<T>
{
    public Range(T from, T to)
    {
        From = from;
        To = to;
    }

    public T From { get; set; }
    public T To { get; set; }

    public bool HasFrom()
    {
        return !Equals(From, default(T));
    }

    public bool HasTo()
    {
        return !Equals(To, default(T));
    }
}

public class OptionalDateRange : Range<DateTime?>
{
    public OptionalDateRange() : base(null, null) { }

    public OptionalDateRange(DateTime? from, DateTime? to) : base(from, to)
    {
    }
}
