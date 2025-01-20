using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Verify;

public class VerifyConfigTests : SnapshotCreatorTestsBase
{
    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", "expected-pool-config.heavy.json")]
    [InlineData("PerformanceTestData.xlsx", "light", "expected-pool-config.light.json")]
    [InlineData("PerformanceTestData.xlsx", "test", "expected-pool-config.test.json")]
    public async Task CommandHandler_ReturnsTrue_WhenValidationSucceeds(string excelFile, string loadTestTag, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigExcelFile = GetFullFilePath(excelFile);
        var poolConfigJsonFile = GetFullFilePath(expectedPoolConfigJsonFilename);
        var command = new VerifyConfig.Command(poolConfigExcelFile, loadTestTag, poolConfigJsonFile);

        var poolConfigurationJsonValidator = new PoolConfigurationJsonValidator();
        var poolConfigurationExcelReader = new PoolConfigurationExcelReader(new ExcelReader());
        var poolConfigurationJsonReader = new PoolConfigurationJsonReader();
        var relationshipAndMessagesGenerator = new RelationshipAndMessagesGenerator();

        var sut = new VerifyConfig.CommandHandler(
            poolConfigurationExcelReader,
            poolConfigurationJsonReader,
            relationshipAndMessagesGenerator,
            poolConfigurationJsonValidator);
        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CommandHandler_ReturnsFalse_WhenValidationFails()
    {
        // Arrange
        var command = new VerifyConfig.Command("path/to/excel", "test", "path/to/json");

        var poolConfigurationExcelReader = A.Fake<IPoolConfigurationExcelReader>();
        var poolConfigurationJsonReader = A.Fake<IPoolConfigurationJsonReader>();
        var relationshipAndMessagesGenerator = A.Fake<IRelationshipAndMessagesGenerator>();
        var poolConfigurationJsonValidator = A.Fake<IPoolConfigurationJsonValidator>();

        var sut = new VerifyConfig.CommandHandler(
            poolConfigurationExcelReader,
            poolConfigurationJsonReader,
            relationshipAndMessagesGenerator,
            poolConfigurationJsonValidator);

        A.CallTo(() => poolConfigurationExcelReader.Read(A<string>._, A<string>._)).Returns(new PerformanceTestConfiguration(new List<PoolConfiguration>(), new VerificationConfiguration()));
        A.CallTo(() => relationshipAndMessagesGenerator.Generate(A<PerformanceTestConfiguration>._)).Returns([]);
        A.CallTo(() => poolConfigurationJsonReader.Read(A<string>._)).Returns(null! as PerformanceTestConfiguration);
        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
