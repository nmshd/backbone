using System.Text.Json;

namespace Backbone.ConsumerApi;

public class VersionService
{
    private const string FILE_PATH = "../../../../AdminApi/src/AdminApi/ClientApp/package-lock.json";

    public async Task<string?> GetDependencyMajorVersionAsync()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(baseDirectory, FILE_PATH);

        await using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        using var document = await JsonDocument.ParseAsync(stream);

        document.RootElement.TryGetProperty("packages", out var packages);
        packages.TryGetProperty("node_modules/less/node_modules/make-dir", out var nodeModulesLessNodeModulesMakeDir);
        nodeModulesLessNodeModulesMakeDir.TryGetProperty("dependencies", out var dependencies);
        dependencies.TryGetProperty("semver", out var semver);

        var majorVersion = semver.ToString();

        int caretIndex = majorVersion.IndexOf('^');
        int dotIndex = majorVersion.IndexOf('.', caretIndex);

        return majorVersion.Substring(caretIndex + 1, dotIndex - caretIndex - 1);
    }
}
