using System.Text.Json;

namespace Backbone.ConsumerApi;

public class VersionService
{
    private const string FILE_PATH = "C:\\Users\\htotbagi\\Code\\backbone\\AdminApi\\src\\AdminApi\\ClientApp\\package-lock.json";

    public async Task<string?> GetDependencyMajorVersionAsync()
    {
        await using var stream = new FileStream(FILE_PATH, FileMode.Open, FileAccess.Read);
        using var document = await JsonDocument.ParseAsync(stream);

        document.RootElement.TryGetProperty("packages", out var packages);
        packages.TryGetProperty("node_modules/less/node_modules/make-dir", out var nodeModulesLessNodeModulesMakeDir);
        nodeModulesLessNodeModulesMakeDir.TryGetProperty("dependencies", out var dependencies);
        dependencies.TryGetProperty("semver", out var semver);

        return semver.ToString();
    }
}
