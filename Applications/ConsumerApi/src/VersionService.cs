namespace Backbone.ConsumerApi;

public class VersionService
{
    public string GetBackboneMajorVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;

        if (string.IsNullOrEmpty(version))
            throw new InvalidOperationException("The file version information could not be retrieved.");

        var majorVersion = version.Split('.', 2)[0];

        return majorVersion;
    }
}
