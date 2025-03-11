namespace Backbone.Tooling.Extensions;

public static class IntegerExtensions
{
    public static int Kibibytes(this int bytes)
    {
        return bytes * 1024;
    }

    public static int Mebibytes(this int bytes)
    {
        return bytes * 1024 * 1024;
    }

    public static int Gibibytes(this int bytes)
    {
        return bytes * 1024 * 1024 * 1024;
    }

    public static TimeSpan Seconds(this int number)
    {
        return TimeSpan.FromSeconds(number);
    }

    public static TimeSpan Minutes(this int number)
    {
        return TimeSpan.FromMinutes(number);
    }
}
