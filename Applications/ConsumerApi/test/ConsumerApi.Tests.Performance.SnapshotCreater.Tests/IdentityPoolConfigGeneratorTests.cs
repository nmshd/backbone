using Xunit.Abstractions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreater.Tests;

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
    [InlineData("PerformanceTestData.xlsx", "heavy-loadtest", "pool-config.heavy.json")]
    [InlineData("PerformanceTestData.xlsx", "light-loadtest", "pool-config.light.json")]
    [InlineData("PerformanceTestData.xlsx", "test-loadtest", "pool-config.test.json")]
    public void VerifyJsonPoolConfig_InputPerformanceTestDataExcel_Success(string excelFile, string workSheet, string expectedLoadTestJson)
    {
        // Arrange
        var expectedJson = Path.Combine(_testDataFolder, expectedLoadTestJson);

        var inputFile = Path.Combine(_testDataFolder, excelFile);

        var currentWorksheet = workSheet;
        
        // Act

        // Assert
    }

    #endregion
}
