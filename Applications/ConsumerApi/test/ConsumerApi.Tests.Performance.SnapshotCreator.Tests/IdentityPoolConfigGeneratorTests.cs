using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;
using FluentAssertions;
using Xunit.Abstractions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests;

public class IdentityPoolConfigGeneratorTests
{
    private readonly string _baseDirectory;
    private readonly string _testDataFolder;

    private readonly ITestOutputHelper _testOutputHelper;

    public IdentityPoolConfigGeneratorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        _baseDirectory = AppContext.BaseDirectory.EndsWith($"\\") ? AppContext.BaseDirectory : $"{AppContext.BaseDirectory}\\";
        _testDataFolder = $@"{_baseDirectory}\TestData";
    }

    #region Verify Json Pool Config

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", "pool-config.heavy.json")]
    [InlineData("PerformanceTestData.xlsx", "light", "pool-config.light.json")]
    [InlineData("PerformanceTestData.xlsx", "test", "pool-config.test.json")]
    public void VerifyJsonPoolConfig_InputPerformanceTestDataExcel_ReturnsSuccess(string excelFile, string workSheet, string expectedLoadTestJson)
    {
        // Arrange
        var expectedJson = Path.Combine(_testDataFolder, expectedLoadTestJson);
        var inputFile = Path.Combine(_testDataFolder, excelFile);
       
        var sut = new IdentityPoolConfigGenerator();

        // Act
        var result = sut.VerifyJsonPoolConfig(inputFile, workSheet, expectedJson);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
