using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Readers;

public class PoolConfigurationExcelReaderTests : SnapshotCreatorTestsBase
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
        expectedPoolConfig.Should().NotBeNull();
        expectedPoolConfig!.RelationshipAndMessages.Clear(); //Note: Excel reader does only read the pool config, not the relationships and messages

        var sut = new PoolConfigurationExcelReader();

        // Act
        var actualPoolConfig = await sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        actualPoolConfig.Should().NotBeNull();
        actualPoolConfig.Should().BeEquivalentTo(expectedPoolConfig);
    }

    #endregion
}
