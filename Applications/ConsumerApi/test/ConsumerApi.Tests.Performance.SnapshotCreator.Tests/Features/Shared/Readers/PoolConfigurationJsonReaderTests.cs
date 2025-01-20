using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Shared.Readers;

public class PoolConfigurationJsonReaderTests : SnapshotCreatorTestsBase
{
    [Theory]
    [InlineData("pool-config.heavy.json", "expected-pool-config.heavy.json")]
    [InlineData("pool-config.light.json", "expected-pool-config.light.json")]
    [InlineData("pool-config.test.json", "expected-pool-config.test.json")]
    public async Task Read_InputPoolConfigJson_ReturnsPoolConfiguration(string poolConfigJsonFilename, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigJsonFile = Path.Combine(TestDataFolder, poolConfigJsonFilename);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        expectedPoolConfig.Should().NotBeNull();
        var sut = new PoolConfigurationJsonReader();

        // Act
        var actualPoolConfig = await sut.Read(poolConfigJsonFile);

        // Assert
        actualPoolConfig.Should().NotBeNull();
        actualPoolConfig.Should().BeEquivalentTo(expectedPoolConfig);
    }

    [Fact]
    public async Task Read_InvalidFileExtension_ShouldThrowException()
    {
        // Arrange
        var poolConfigJsonFile = Path.Combine(TestDataFolder, "expected-pool-config.test.jinx");
        var sut = new PoolConfigurationJsonReader();

        // Act
        var act = () => sut.Read(poolConfigJsonFile);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
