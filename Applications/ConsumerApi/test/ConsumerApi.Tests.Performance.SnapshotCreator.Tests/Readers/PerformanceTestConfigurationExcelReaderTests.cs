using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Readers;

public class PerformanceTestConfigurationExcelReaderTests : SnapshotCreatorTestsBase
{
    #region Verify Json Pool Config

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", "expected-pool-config.heavy.json")]
    [InlineData("PerformanceTestData.xlsx", "light", "expected-pool-config.light.json")]
    [InlineData("PerformanceTestData.xlsx", "test", "expected-pool-config.test.json")]
    public async Task Read_InputPerformanceTestDataExcel_ReturnsPoolConfiguration(string excelFile, string loadTestTag, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigExcelFile = Path.Combine(TestDataFolder, excelFile);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        expectedPoolConfig.RelationshipAndMessages.Clear(); //Note: Excel reader does only read the pool config, not the relationships and messages
        var sut = new PerformanceTestConfigurationExcelReader();

        // Act
        var actualPoolConfig = await sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        actualPoolConfig.Should().NotBeNull();
        actualPoolConfig.Should().BeEquivalentTo(expectedPoolConfig);
    }

    #endregion
}
