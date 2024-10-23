using System.Diagnostics;
using System.Reflection;

namespace Backbone.ConsumerApi;

public class VersionService
{
    public static string GetBackboneMajorVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;

        if (string.IsNullOrEmpty(version))
            throw new InvalidOperationException("The file version information could not be retrieved.");

        var majorVersion = version.Split('.', 2)[0];

        return majorVersion;
    }
}
