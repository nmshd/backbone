using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Generate;

public class GenerateConfigTests : SnapshotCreatorTestsBase
{
    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", false)]
    [InlineData("PerformanceTestData.xlsx", "light", false)]
    [InlineData("PerformanceTestData.xlsx", "test", false)]
    [InlineData("PerformanceTestData.xlsx", "heavy", true)]
    [InlineData("PerformanceTestData.xlsx", "light", true)]
    [InlineData("PerformanceTestData.xlsx", "test", true)]
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
        if (Directory.Exists(result.PoolConfigurationFolder))
        {
            Directory.Delete(result.PoolConfigurationFolder, true);
        }
    }


    [Fact]
    public async Task CommandHandler_WhenWriteJsonPoolConfigurationsFailed_ShouldThrowException()
    {
        // Arrange
        var command = new GenerateConfig.Command(GetFullFilePath("PerformanceTestData.xlsx"), "test", true);

        var poolConfigurationExcelReader = A.Fake<IPoolConfigurationExcelReader>();
        var relationshipAndMessagesGenerator = A.Fake<IRelationshipAndMessagesGenerator>();
        var poolConfigurationJsonWriter = A.Fake<IPoolConfigurationJsonWriter>();
        var excelWriter = A.Fake<IExcelWriter>();
        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).Returns(new StatusMessage(false, "Failed", new Exception("Failed")));

        var sut = new GenerateConfig.CommandHandler(poolConfigurationExcelReader, relationshipAndMessagesGenerator, poolConfigurationJsonWriter, excelWriter);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).MustHaveHappenedOnceExactly();
        result.Status.Should().BeFalse();

        if (Directory.Exists(result.PoolConfigurationFolder))
        {
            Directory.Delete(result.PoolConfigurationFolder!, true);
        }
    }

    [Fact]
    public async Task CommandHandler_WhenWritePoolConfigurationsFailed_ShouldThrowException()
    {
        // Arrange
        var command = new GenerateConfig.Command(GetFullFilePath("PerformanceTestData.xlsx"), "test", true);

        var poolConfigurationExcelReader = A.Fake<IPoolConfigurationExcelReader>();
        var relationshipAndMessagesGenerator = A.Fake<IRelationshipAndMessagesGenerator>();
        var poolConfigurationJsonWriter = A.Fake<IPoolConfigurationJsonWriter>();
        var excelWriter = A.Fake<IExcelWriter>();
        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).Returns(Task.FromResult(new StatusMessage(true, "Success")));
        A.CallTo(() => excelWriter.WritePoolConfigurations(A<string>._, A<string>._, A<List<PoolConfiguration>>._)).Throws<Exception>();


        var sut = new GenerateConfig.CommandHandler(poolConfigurationExcelReader, relationshipAndMessagesGenerator, poolConfigurationJsonWriter, excelWriter);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => excelWriter.WritePoolConfigurations(A<string>._, A<string>._, A<List<PoolConfiguration>>._)).MustHaveHappenedOnceExactly();
        result.Status.Should().BeFalse();

        if (Directory.Exists(result.PoolConfigurationFolder))
        {
            Directory.Delete(result.PoolConfigurationFolder!, true);
        }
    }

    [Fact]
    public async Task CommandHandler_WhenWriteRelationshipsAndMessagesFailed_ShouldThrowException()
    {
        // Arrange
        var command = new GenerateConfig.Command(GetFullFilePath("PerformanceTestData.xlsx"), "test", true);

        var poolConfigurationExcelReader = A.Fake<IPoolConfigurationExcelReader>();
        var relationshipAndMessagesGenerator = A.Fake<IRelationshipAndMessagesGenerator>();
        var poolConfigurationJsonWriter = A.Fake<IPoolConfigurationJsonWriter>();
        var excelWriter = A.Fake<IExcelWriter>();
        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).Returns(Task.FromResult(new StatusMessage(true, "Success")));
        A.CallTo(() => excelWriter.WriteRelationshipsAndMessages(A<string>._, A<string>._, A<List<RelationshipAndMessages>>._)).Throws<Exception>();


        var sut = new GenerateConfig.CommandHandler(poolConfigurationExcelReader, relationshipAndMessagesGenerator, poolConfigurationJsonWriter, excelWriter);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => excelWriter.WriteRelationshipsAndMessages(A<string>._, A<string>._, A<List<RelationshipAndMessages>>._)).MustHaveHappenedOnceExactly();
        result.Status.Should().BeFalse();

        if (Directory.Exists(result.PoolConfigurationFolder))
        {
            Directory.Delete(result.PoolConfigurationFolder!, true);
        }
    }

    [Fact]
    public async Task CommandHandler_WhenWriteSucceeds_ShouldReturnSuccessStatusMessage()
    {
        // Arrange
        var command = new GenerateConfig.Command(GetFullFilePath("PerformanceTestData.xlsx"), "test", true);

        var poolConfigurationExcelReader = A.Fake<IPoolConfigurationExcelReader>();
        var relationshipAndMessagesGenerator = A.Fake<IRelationshipAndMessagesGenerator>();
        var poolConfigurationJsonWriter = A.Fake<IPoolConfigurationJsonWriter>();
        var excelWriter = A.Fake<IExcelWriter>();

        A.CallTo(() => poolConfigurationJsonWriter.Write(A<PerformanceTestConfiguration>._, A<string>._)).Returns(Task.FromResult(new StatusMessage(true, "Success")));
        A.CallTo(() => excelWriter.WritePoolConfigurations(A<string>._, A<string>._, A<List<PoolConfiguration>>._)).Returns(Task.CompletedTask);
        A.CallTo(() => excelWriter.WriteRelationshipsAndMessages(A<string>._, A<string>._, A<List<RelationshipAndMessages>>._)).Returns(Task.CompletedTask);

        var sut = new GenerateConfig.CommandHandler(poolConfigurationExcelReader, relationshipAndMessagesGenerator, poolConfigurationJsonWriter, excelWriter);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => excelWriter.WritePoolConfigurations(A<string>._, A<string>._, A<List<PoolConfiguration>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => excelWriter.WriteRelationshipsAndMessages(A<string>._, A<string>._, A<List<RelationshipAndMessages>>._)).MustHaveHappenedOnceExactly();
        result.Status.Should().BeTrue();

        if (Directory.Exists(result.PoolConfigurationFolder))
        {
            Directory.Delete(result.PoolConfigurationFolder!, true);
        }
    }
}
