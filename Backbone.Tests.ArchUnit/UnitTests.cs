using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ArchUnitNET.xUnitV3;
using Backbone.UnitTestTools.BaseClasses;
using Shouldly;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class UnitTests
{
    [Fact]
    public void UnitTestFoldersAndNamespacesShouldMirrorSourceFolders()
    {
        foreach (var (testFile, testDirectory, sourceDirectory, rootNamespace) in GetUnitTestFiles())
        {
            var relativeFolder = Path.GetRelativePath(testDirectory, Path.GetDirectoryName(testFile)!);
            var sourceFolder = Path.Combine(sourceDirectory, relativeFolder);
            Directory.Exists(sourceFolder).ShouldBeTrue($"{testFile} must mirror an existing source folder.");

            var expectedNamespace = relativeFolder == "." ? rootNamespace : rootNamespace + "." + relativeFolder.Replace(Path.DirectorySeparatorChar, '.');
            var namespaceMatch = Regex.Match(File.ReadAllText(testFile), @"(?m)^namespace\s+([^;{]+)");
            namespaceMatch.Success.ShouldBeTrue($"{testFile} must declare a namespace.");
            namespaceMatch.Groups[1].Value.Trim().ShouldBe(expectedNamespace, testFile);
        }
    }

    [Fact]
    public void DomainEventHandlerTestsShouldBeNamedHandlerTests()
    {
        foreach (var (testFile, testDirectory, sourceDirectory, _) in GetUnitTestFiles())
        {
            var relativeFolder = Path.GetRelativePath(testDirectory, Path.GetDirectoryName(testFile)!);
            var handlerFile = Path.Combine(sourceDirectory, relativeFolder, "Handler.cs");
            if (!File.Exists(handlerFile) || !File.ReadAllText(handlerFile).Contains("IDomainEventHandler<"))
                continue;

            Path.GetFileName(testFile).ShouldBe("HandlerTests.cs");
            Regex.Match(File.ReadAllText(testFile), @"\bclass\s+(\w+)").Groups[1].Value.ShouldBe("HandlerTests", testFile);
        }
    }

    [Fact]
    public void UnitTestsShouldExtendAbstractTestsBase()
    {
        Classes().That().HaveNameMatching(".+Tests$")
            .And().AreNot(typeof(UnitTests))
            .Should().BeAssignableTo(typeof(AbstractTestsBase))
            .Check(Backbone.ARCHITECTURE);
    }

    private static IEnumerable<(string TestFile, string TestDirectory, string SourceDirectory, string RootNamespace)> GetUnitTestFiles([CallerFilePath] string callerFile = "")
    {
        var repositoryDirectory = Path.GetDirectoryName(Path.GetDirectoryName(callerFile))!;
        var projects = Directory.GetFiles(repositoryDirectory, "*.csproj", SearchOption.AllDirectories);
        var sourceProjects = projects.Where(p => p.Contains($"{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}"))
            .ToDictionary(p => Path.GetFileNameWithoutExtension(p)!, p => p);

        foreach (var project in projects.Where(p => p.Contains($"{Path.DirectorySeparatorChar}test{Path.DirectorySeparatorChar}")))
        {
            var name = Path.GetFileNameWithoutExtension(project);
            if (!name.EndsWith("Tests"))
                continue;

            var sourceName = name.EndsWith(".Tests") ? name[..^6] : name[..^5];
            if (!sourceProjects.TryGetValue(sourceName, out var sourceProject))
                continue;

            var testDirectory = Path.GetDirectoryName(project)!;
            var sourceDirectory = Path.GetDirectoryName(sourceProject)!;
            var rootNamespace = project.StartsWith(Path.Combine(repositoryDirectory, "Modules") + Path.DirectorySeparatorChar)
                ? "Backbone.Modules." + name
                : "Backbone." + name;

            foreach (var testFile in Directory.GetFiles(testDirectory, "*.cs", SearchOption.AllDirectories))
            {
                var pathParts = Path.GetRelativePath(testDirectory, testFile).Split(Path.DirectorySeparatorChar);
                if (pathParts.Contains("bin") || pathParts.Contains("obj") || !Regex.IsMatch(File.ReadAllText(testFile), @"\[(Fact|Theory)\b"))
                    continue;

                yield return (testFile, testDirectory, sourceDirectory, rootNamespace);
            }
        }
    }
}
