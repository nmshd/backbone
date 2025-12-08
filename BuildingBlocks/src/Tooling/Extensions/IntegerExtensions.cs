namespace Backbone.Tooling.Extensions;

public static class IntegerExtensions
{
    extension(int bytes)
    {
        public int Kibibytes()
        {
            return bytes * 1024;
        }

        public int Mebibytes()
        {
            return bytes * 1024 * 1024;
        }

        public int Gibibytes()
        {
            return bytes * 1024 * 1024 * 1024;
        }

        public TimeSpan Seconds()
        {
            return TimeSpan.FromSeconds(bytes);
        }

        public TimeSpan Minutes()
        {
            return TimeSpan.FromMinutes(bytes);
        }
    }
}
