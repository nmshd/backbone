using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Validators;
using FluentAssertions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Validators;

public class PoolConfigurationJsonValidatorTests : SnapshotCreatorTestsBase
{
    #region Verify Json Pool Config

    [Theory]
    [InlineData("heavy")]
    [InlineData("light")]
    [InlineData("test")]
    public async Task VerifyPoolConfig_InputPerformanceTestDataExcel_ReturnsSuccess(string loadTestTag)
    {
        // Arrange
        var sut = new PoolConfigurationJsonValidator();

        var performanceTestConfiguration = GetExpectedPoolConfiguration(loadTestTag);

        // Act
        var result = await sut.Validate(performanceTestConfiguration, performanceTestConfiguration);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
