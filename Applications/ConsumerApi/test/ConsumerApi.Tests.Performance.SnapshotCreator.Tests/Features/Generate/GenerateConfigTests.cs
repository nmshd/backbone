using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Generate;

public class GenerateConfigTests : SnapshotCreatorTestsBase
{
    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", false)]
    [InlineData("PerformanceTestData.xlsx", "light", false)]
    [InlineData("PerformanceTestData.xlsx", "test", false)]
    public async Task CommandHandler_WhenExecutionIsSuccessful_ShouldReturnSuccessStatusMessage(string excelPoolConfig, string loadTestTag, bool isDebug)
    {
        // Arrange
        var fullExcelPath = GetFullFilePath(excelPoolConfig);
        var command = new GenerateConfig.Command(fullExcelPath, loadTestTag, isDebug);

        var excelWriter = new ExcelWriter();
        var poolConfigurationExcelReader = new PoolConfigurationExcelReader(new ExcelReader());
        var relationshipAndMessagesGenerator = new RelationshipAndMessagesGenerator();
        var poolConfigurationJsonWriter = new PoolConfigurationJsonWriter();

        var sut = new GenerateConfig.CommandHandler(poolConfigurationExcelReader, relationshipAndMessagesGenerator, poolConfigurationJsonWriter, excelWriter);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().BeTrue();
    }
}
