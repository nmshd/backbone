using System.Text.Json;

namespace Backbone.ConsumerApi;

public class VersionService
{
    private static readonly string FILE_PATH = AppDomain.CurrentDomain.BaseDirectory + "../../../../AdminApi/src/AdminApi/ClientApp/package-lock.json";

    public async Task<string?> GetCurrentBackboneVersion()
    {
        await using var stream = new FileStream(FILE_PATH, FileMode.Open, FileAccess.Read);
        using var document = await JsonDocument.ParseAsync(stream);

        document.RootElement.TryGetProperty("packages", out var packages);
        packages.TryGetProperty("node_modules/less/node_modules/make-dir", out var nodeModulesLessNodeModulesMakeDir);
        nodeModulesLessNodeModulesMakeDir.TryGetProperty("dependencies", out var dependencies);
        dependencies.TryGetProperty("semver", out var semver);

        var majorVersion = semver.ToString();

        var caretIndex = majorVersion.IndexOf('^');
        var dotIndex = majorVersion.IndexOf('.', caretIndex);

        return majorVersion.Substring(caretIndex + 1, dotIndex - caretIndex - 1);
    }
}
