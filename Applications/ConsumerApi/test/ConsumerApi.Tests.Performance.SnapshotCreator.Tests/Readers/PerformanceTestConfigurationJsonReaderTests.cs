using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using FluentAssertions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Readers;

public class PerformanceTestConfigurationJsonReaderTests : SnapshotCreatorTestsBase
{
    #region Deserialize From Json Tests

    [Theory]
    [InlineData("pool-config.heavy.json", "heavy")]
    [InlineData("pool-config.light.json", "light")]
    [InlineData("pool-config.test.json", "test")]
    public async Task Read_InputPoolConfigJson_ReturnsPoolConfiguration(string poolConfigJsonFilename, string loadTestTag)
    {
        // Arrange
        var poolConfigJsonFile = Path.Combine(TestDataFolder, poolConfigJsonFilename);
        var expectedPoolConfig = GetExpectedPoolConfiguration(loadTestTag);
        var sut = new PerformanceTestConfigurationJsonReader();

        // Act
        var actualPoolConfig = await sut.Read(poolConfigJsonFile);

        // Assert
        actualPoolConfig.Should().NotBeNull();

        var areEqual = actualPoolConfig.Equals(expectedPoolConfig); //Note: Should().BeEquivalentTo does not invoke overridden Equals method
        areEqual.Should().BeTrue();
    }

    #endregion
}
