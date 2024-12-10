using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;
using FakeItEasy;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Shared.Readers;

public class PoolConfigurationExcelReaderTests : SnapshotCreatorTestsBase
{
    private readonly IExcelReader _excelReader;
    private readonly PoolConfigurationExcelReader _sut;

    public PoolConfigurationExcelReaderTests()
    {
        _excelReader = A.Fake<IExcelReader>();
        _sut = new PoolConfigurationExcelReader(_excelReader);
    }

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", "expected-pool-config.heavy.json")]
    [InlineData("PerformanceTestData.xlsx", "light", "expected-pool-config.light.json")]
    [InlineData("PerformanceTestData.xlsx", "test", "expected-pool-config.test.json")]
    public async Task Read_InputPerformanceTestDataExcel_ReturnsPoolConfiguration(string excelFile, string loadTestTag, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigExcelFile = Path.Combine(TestDataFolder, excelFile);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        expectedPoolConfig.Should().NotBeNull();
        expectedPoolConfig!.RelationshipAndMessages.Clear(); //Note: Excel reader does only read the pool config, not the relationships and messages

        var sut = new PoolConfigurationExcelReader(new ExcelReader());

        // Act
        var actualPoolConfig = await sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        actualPoolConfig.Should().NotBeNull();
        actualPoolConfig.Should().BeEquivalentTo(expectedPoolConfig);
    }


    [Theory]
    [InlineData("PerformanceTestData.xlsx", "test", "expected-pool-config.test.json")]
    public async Task Read_MaterializedPoolConfigCountIsZero_ShouldThrowException(string excelFile, string loadTestTag, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigExcelFile = Path.Combine(TestDataFolder, excelFile);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        expectedPoolConfig.Should().NotBeNull();
        expectedPoolConfig!.RelationshipAndMessages.Clear(); //Note: Excel reader does only read the pool config, not the relationships and messages

        A.CallTo(() => _excelReader.FetchAsync(A<string>._, A<ExcelMapper>._, A<FileStream>._)).Returns(new List<dynamic>());

        // Act
        var act = () => _sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(PERFORMANCE_TEST_CONFIGURATION_EXCEL_FILE_EMPTY);
    }

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "test", "expected-pool-config.test.json")]
    public async Task Read_MaterializedPoolConfigFirstHasWrongType_ShouldThrowException(string excelFile, string loadTestTag, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigExcelFile = Path.Combine(TestDataFolder, excelFile);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        expectedPoolConfig.Should().NotBeNull();
        expectedPoolConfig!.RelationshipAndMessages.Clear(); //Note: Excel reader does only read the pool config, not the relationships and messages

        var poolConfigFromExcel = new List<dynamic>
        {
            new Dictionary<int, object>()
        };

        A.CallTo(() => _excelReader.FetchAsync(A<string>._, A<ExcelMapper>._, A<FileStream>._)).Returns(poolConfigFromExcel);

        // Act
        var act = () => _sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"{PERFORMANCE_TEST_CONFIGURATION_FIRST_ROW_MISMATCH} {nameof(IDictionary<string, object>)}");
    }


    [Theory]
    [InlineData("PerformanceTestData.xlsx", "test", "expected-pool-config.test.json")]
    public async Task Read_SkipExcelDataWithWrongType_ShouldContinueAndReturnPoolConfiguration(string excelFile, string loadTestTag, string expectedPoolConfigJsonFilename)
    {
        // Arrange
        var poolConfigExcelFile = Path.Combine(TestDataFolder, excelFile);
        var expectedPoolConfig = await GetExpectedPoolConfiguration(expectedPoolConfigJsonFilename);
        expectedPoolConfig.Should().NotBeNull();
        expectedPoolConfig!.RelationshipAndMessages.Clear(); //Note: Excel reader does only read the pool config, not the relationships and messages

        var expectedPoolConfiguration = new PoolConfiguration
        {
            Type = POOL_TYPE_APP,
            Name = "app",
            Alias = "a1",
            Amount = 1,
            NumberOfRelationshipTemplates = 1,
            NumberOfRelationships = 1,
            NumberOfSentMessages = 1,
            NumberOfReceivedMessages = 1,
            NumberOfDatawalletModifications = 1,
            NumberOfDevices = 1,
            NumberOfChallenges = 1
        };

        var poolConfigFromExcel = new List<dynamic>
        {
            new Dictionary<string, object>
            {
                { nameof(PoolConfiguration.Type), expectedPoolConfiguration.Type },
                { nameof(PoolConfiguration.Name), expectedPoolConfiguration.Name },
                { nameof(PoolConfiguration.Alias), expectedPoolConfiguration.Alias },
                { nameof(PoolConfiguration.Amount), expectedPoolConfiguration.Amount },
                { nameof(PoolConfiguration.NumberOfRelationshipTemplates), expectedPoolConfiguration.NumberOfRelationshipTemplates },
                { nameof(PoolConfiguration.NumberOfRelationships), expectedPoolConfiguration.NumberOfRelationships },
                { nameof(PoolConfiguration.NumberOfSentMessages), expectedPoolConfiguration.NumberOfSentMessages },
                { nameof(PoolConfiguration.NumberOfReceivedMessages), expectedPoolConfiguration.NumberOfReceivedMessages },
                { nameof(PoolConfiguration.NumberOfDatawalletModifications), expectedPoolConfiguration.NumberOfDatawalletModifications },
                { nameof(PoolConfiguration.NumberOfDevices), expectedPoolConfiguration.NumberOfDevices },
                { nameof(PoolConfiguration.NumberOfChallenges), expectedPoolConfiguration.NumberOfChallenges },
                { TOTAL_NUMBER_OF_RELATIONSHIPS, 1 },
                { APP_TOTAL_NUMBER_OF_SENT_MESSAGES, 1 },
                { CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES, 1 }
            },
            new Dictionary<int, object>()
        };

        A.CallTo(() => _excelReader.FetchAsync(A<string>._, A<ExcelMapper>._, A<FileStream>._)).Returns(poolConfigFromExcel);

        // Act
        var result = await _sut.Read(poolConfigExcelFile, loadTestTag);

        // Assert
        result.Should().NotBeNull();
        result.PoolConfigurations.Should().HaveCount(1);
        result.PoolConfigurations.First().Should().BeEquivalentTo(expectedPoolConfiguration);
    }
}
