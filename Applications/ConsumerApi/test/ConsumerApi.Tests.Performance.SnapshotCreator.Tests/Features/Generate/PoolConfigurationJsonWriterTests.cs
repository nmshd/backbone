using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Generate;

public class PoolConfigurationJsonWriterTests : SnapshotCreatorTestsBase
{
    [Fact]
    public async Task Write_TestConfiguration_ReturnsTrue()
    {
        var filePath = GetFullFilePath($"poolConfiguration-{Guid.NewGuid()}.json");
        try
        {
            // Arrange
            var expectedResult = new StatusMessage(true, Path.GetFullPath(filePath));
            var sut = new PoolConfigurationJsonWriter();

            // Act
            var result = await sut.Write(new PerformanceTestConfiguration(new List<PoolConfiguration>(), new VerificationConfiguration()), filePath);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task Write_InvalidFileNamesNull_ReturnsFalse()
    {
        var filePath = GetFullFilePath($"poolConfiguration-{DateTime.Now:O}.json");

        // Arrange
        var sut = new PoolConfigurationJsonWriter();

        // Act
        var result = await sut.Write(null!, filePath);

        // Assert
        result.Status.Should().BeFalse();
    }
}
