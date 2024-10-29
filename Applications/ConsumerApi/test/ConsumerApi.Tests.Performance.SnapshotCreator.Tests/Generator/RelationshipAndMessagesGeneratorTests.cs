using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Generator;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using FluentAssertions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Generator;

public class RelationshipAndMessagesGeneratorTests : SnapshotCreatorTestsBase
{
    #region Generate Excel RelationshipsAndMessages Pool Config

    [Theory]
    [InlineData("test", "ExpectedRelationshipsAndMessagePoolConfigs.test.json")]
    [InlineData("light", "ExpectedRelationshipsAndMessagePoolConfigs.light.json")]
    [InlineData("heavy", "ExpectedRelationshipsAndMessagePoolConfigs.heavy.json")]
    public async Task Generate_InputPerformanceTestData_ReturnsSuccess(string workSheet, string expectedLoadTestJsonFile)
    {
        // Arrange
        var expectedLoadTestDataFilePath = Path.Combine(TestDataFolder, expectedLoadTestJsonFile);

        await using var fileStream = File.OpenRead(expectedLoadTestDataFilePath);
        var expectedLoadTestData = await System.Text.Json.JsonSerializer.DeserializeAsync<RelationshipAndMessages[]>(fileStream);


        var poolConfigJson = GetExpectedPoolConfiguration(workSheet);

        var sut = new RelationshipAndMessagesGenerator();

        // Act
        var actualLoadTestData = sut.Generate(poolConfigJson);

        // Assert
        actualLoadTestData.SequenceEqual(expectedLoadTestData).Should().BeTrue();
    }

    #endregion
}
