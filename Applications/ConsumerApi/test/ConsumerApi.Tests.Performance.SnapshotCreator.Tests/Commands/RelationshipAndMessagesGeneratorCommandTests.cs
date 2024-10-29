using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands.Args;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Generator;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;
using FluentAssertions;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Commands;

public class RelationshipAndMessagesGeneratorCommandTests : SnapshotCreatorTestsBase
{
    #region Generate Excel RelationshipsAndMessages Pool Config

    [Theory]
    [InlineData("test", "pool-config.test.json", "ExpectedRelationshipsAndMessagePoolConfigs.test.json")]
    [InlineData("light", "pool-config.light.json", "ExpectedRelationshipsAndMessagePoolConfigs.light.json")]
    [InlineData("heavy", "pool-config.heavy.json", "ExpectedRelationshipsAndMessagePoolConfigs.heavy.json")]
    public async Task Execute_InputPoolConfigJsonFile_ReturnsSuccess(string workSheet, string poolConfigJsonFilename, string expectedLoadTestJsonFile)
    {
        // Arrange
        var poolConfigJsonFilePath = Path.Combine(TestDataFolder, poolConfigJsonFilename);
        var expectedRelationshipsFilePath = Path.Combine(TestDataFolder, $"{RELATIONSHIPS_AND_MESSAGE_POOL_CONFIGS_FILE_NAME}.{workSheet}.{EXCEL_FILE_EXT}");
        var expectedLoadTestDataFilePath = Path.Combine(TestDataFolder, expectedLoadTestJsonFile);

        await using var fileStream = File.OpenRead(expectedLoadTestDataFilePath);
        var expectedLoadTestData = await System.Text.Json.JsonSerializer.DeserializeAsync<RelationshipAndMessages[]>(fileStream);

        var performanceConfigJsonReader = new PerformanceTestConfigurationJsonReader();
        var relationshipAndMessagesGenerator = new RelationshipAndMessagesGenerator();
        var relationshipAndMessagesExcelWriter = new RelationshipAndMessagesExcelWriter();

        var sut = new RelationshipAndMessagesGeneratorCommand(
            performanceConfigJsonReader,
            relationshipAndMessagesGenerator,
            relationshipAndMessagesExcelWriter);

        // Act
        var statusMessage = await sut.Execute(new RelationshipAndMessagesGeneratorCommandArgs(poolConfigJsonFilePath, workSheet));

        // Assert

        statusMessage.Status.Should().BeTrue($"The operation should succeed, but failed with message: {statusMessage.Message}");
        statusMessage.Message.Should().Be(expectedRelationshipsFilePath);

        File.Exists(expectedRelationshipsFilePath).Should().BeTrue();

        var actualLoadTestData = await (new ExcelMapper().FetchAsync<RelationshipAndMessages>(expectedRelationshipsFilePath));
        actualLoadTestData.Should().BeEquivalentTo(expectedLoadTestData);
    }

    #endregion
}
