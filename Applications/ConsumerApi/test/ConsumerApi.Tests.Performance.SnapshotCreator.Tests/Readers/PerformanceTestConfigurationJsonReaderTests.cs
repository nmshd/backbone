using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Readers;

public class PerformanceTestConfigurationJsonReaderTests : SnapshotCreatorTestsBase
{
    #region Deserialize From Json Tests

    [Theory]
    [InlineData("pool-config.heavy.json", "expected-pool-config.heavy.json")]
    [InlineData("pool-config.light.json", "expected-pool-config.light.json")]
    [InlineData("pool-config.test.json", "expected-pool-config.test.json")]
    public async Task Read_InputPoolConfigJson_ReturnsPoolConfiguration(string poolConfigJsonFilename, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigJsonFile = Path.Combine(TestDataFolder, poolConfigJsonFilename);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        var sut = new PerformanceTestConfigurationJsonReader();

        // Act
        var actualPoolConfig = await sut.Read(poolConfigJsonFile);

        // Assert
        actualPoolConfig.Should().NotBeNull();
        actualPoolConfig.Should().BeEquivalentTo(expectedPoolConfig);
    }

    #endregion
}
