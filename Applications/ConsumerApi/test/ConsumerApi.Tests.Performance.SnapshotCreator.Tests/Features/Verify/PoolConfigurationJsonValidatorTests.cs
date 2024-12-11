using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Verify;

public class PoolConfigurationJsonValidatorTests
{
    private readonly PoolConfigurationJsonValidator _sut = new();

    [Fact]
    public async Task Validate_ReturnsTrue_WhenConfigurationsAreEqual()
    {
        // Arrange
        var configFromJson = new PerformanceTestConfiguration(new List<PoolConfiguration>(), new VerificationConfiguration());
        var configFromExcel = configFromJson;

        // Act
        var result = await _sut.Validate(configFromJson, configFromExcel);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ReturnsFalse_WhenConfigurationsAreNotEqual()
    {
        // Arrange
        var configFromJson = new PerformanceTestConfiguration(new List<PoolConfiguration>(), new VerificationConfiguration()
        {
            TotalAppSentMessages = 1
        });
        var configFromExcel = new PerformanceTestConfiguration(new List<PoolConfiguration>(), new VerificationConfiguration()
        {
            TotalAppSentMessages = 2
        });

        // Act
        var result = await _sut.Validate(configFromJson, configFromExcel);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_ReturnsFalse_WhenOneConfigurationIsNull()
    {
        // Arrange
        var configFromJson = new PerformanceTestConfiguration(new List<PoolConfiguration>(), new VerificationConfiguration());
        ;
        PerformanceTestConfiguration configFromExcel = null!;

        // Act
        var result = await _sut.Validate(configFromJson, configFromExcel);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_ReturnsFalse_WhenBothConfigurationsAreNull()
    {
        // Arrange
        PerformanceTestConfiguration configFromJson = null!;
        PerformanceTestConfiguration configFromExcel = null!;

        // Act
        var result = await _sut.Validate(configFromJson, configFromExcel);

        // Assert
        result.Should().BeFalse();
    }
}
