using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands.Args;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Validators;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;
using FluentAssertions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Commands;

public class PoolConfigurationJsonGeneratorCommandTests : SnapshotCreatorTestsBase
{
    #region Generate Json Pool Config

    [Theory]
    [InlineData("PerformanceTestData-plusNewMedium.xlsx", WORKBOOK_SHEET_MEDIUM_LOAD, "pool-config.medium.json")]
    public async Task GeneratePoolConfig_InputPerformanceTestDataExcel_ReturnsSuccess(string excelFile, string workSheet, string expectedLoadTestJsonFilename)
    {
        // Arrange
        var expectedPoolConfigJsonFilePath = Path.Combine(TestDataFolder, expectedLoadTestJsonFilename);
        var inputFile = Path.Combine(TestDataFolder, excelFile);

        var poolConfigJsonWriter = new PoolConfigurationJsonWriter();
        var performanceTestConfigurationExcelReader = new PerformanceTestConfigurationExcelReader();

        var sut = new PoolConfigurationJsonGeneratorCommand(performanceTestConfigurationExcelReader, poolConfigJsonWriter);

        // Act
        var statusMessage = await sut.Execute(new PoolConfigurationJsonGeneratorCommandArgs(inputFile, workSheet));

        // Assert

        statusMessage.Status.Should().BeTrue();

        File.Exists(expectedPoolConfigJsonFilePath).Should().BeTrue();

        var poolConfigFromExcel = await performanceTestConfigurationExcelReader.Read(inputFile, workSheet);

        var poolConfigJsonReader = new PerformanceTestConfigurationJsonReader();
        var poolConfigFromJson = await poolConfigJsonReader.Read(expectedPoolConfigJsonFilePath);
        var areEqual = await new PoolConfigurationJsonValidator().Validate(poolConfigFromJson, poolConfigFromExcel);

        areEqual.Should().BeTrue();
    }

    #endregion
}
