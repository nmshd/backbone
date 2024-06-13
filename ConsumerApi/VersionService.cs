using System.Text.Json;

namespace Backbone.ConsumerApi;

public class VersionService
{
    private static readonly string FILE_PATH = AppDomain.CurrentDomain.BaseDirectory + "../../../../AdminApi/src/AdminApi/ClientApp/package-lock.json";

    public async Task<string?> GetBackboneMajorVersion()
    {
        await using var stream = new FileStream(FILE_PATH, FileMode.Open, FileAccess.Read);
        using var jsonDocument = await JsonDocument.ParseAsync(stream);

        jsonDocument.RootElement.TryGetProperty("packages", out var packages);
        packages.TryGetProperty("node_modules/less/node_modules/make-dir", out var nodeModulesLessNodeModulesMakeDir);
        nodeModulesLessNodeModulesMakeDir.TryGetProperty("dependencies", out var dependencies);
        dependencies.TryGetProperty("semver", out var semver);

        var version = semver.ToString();

        var caretIndex = version.IndexOf('^');
        var dotIndex = version.IndexOf('.', caretIndex);

        return version.Substring(caretIndex + 1, dotIndex - caretIndex - 1);
    }
}
