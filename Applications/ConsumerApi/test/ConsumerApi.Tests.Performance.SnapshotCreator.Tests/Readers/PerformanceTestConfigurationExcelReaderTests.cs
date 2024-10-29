using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using FluentAssertions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Readers;

public class PerformanceTestConfigurationExcelReaderTests : SnapshotCreatorTestsBase
{
    #region Verify Json Pool Config

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy")]
    [InlineData("PerformanceTestData.xlsx", "light")]
    [InlineData("PerformanceTestData.xlsx", "test")]
    public async Task Read_InputPerformanceTestDataExcel_ReturnsPoolConfiguration(string excelFile, string loadTestTag)
    {
        // Arrange
        var poolConfigExcelFile = Path.Combine(TestDataFolder, excelFile);
        var expectedPoolConfig = GetExpectedPoolConfiguration(loadTestTag);
        var sut = new PerformanceTestConfigurationExcelReader();

        // Act
        var actualPoolConfig = await sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        actualPoolConfig.Should().NotBeNull();

        var areEqual = actualPoolConfig.Equals(expectedPoolConfig); //Note: Should().BeEquivalentTo does not invoke overridden Equals method
        areEqual.Should().BeTrue();
    }

    #endregion
}
