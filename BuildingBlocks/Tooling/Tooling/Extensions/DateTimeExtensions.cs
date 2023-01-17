namespace Enmeshed.Tooling.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToUniversalString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }
}