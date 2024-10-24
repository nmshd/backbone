using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using FluentAssertions;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests;

public class IdentityPoolConfigGeneratorTests
{
    private readonly string _testDataFolder = Path.Combine(AppContext.BaseDirectory, "TestData");

    #region Helper Methods

    private static PerformanceTestConfiguration GetExpectedPoolConfiguration(string loadTestTag)
    {
        var expectedPoolConfiguration = loadTestTag switch
        {
            WORKBOOK_SHEET_HEAVY_LOAD => new PerformanceTestConfiguration(HeavyLoadIdentityPool, HeavyLoadVerificationConfiguration),
            WORKBOOK_SHEET_LIGHT_LOAD => new PerformanceTestConfiguration(LightLoadIdentityPool, LightLoadVerificationConfiguration),
            WORKBOOK_SHEET_TEST_LOAD => new PerformanceTestConfiguration(TestLoadIdentityPool, TestLoadVerificationConfiguration),
            _ => throw new ArgumentException($"Invalid load test tag: {loadTestTag}", nameof(loadTestTag))
        };

        return expectedPoolConfiguration;
    }

    private static VerificationConfiguration TestLoadVerificationConfiguration =>
        new()
        {
            App = new AppVerificationConfiguration
            {
                TotalNumberOfSentMessages = 48,
                TotalNumberOfReceivedMessages = 45,
                NumberOfReceivedMessagesAddOn = 3,
                TotalNumberOfRelationships = 22
            },
            Connector = new ConnectorVerificationConfiguration
            {
                TotalNumberOfSentMessages = 48,
                TotalNumberOfReceivedMessages = 46,
                NumberOfReceivedMessagesAddOn = 2,
                TotalNumberOfAvailableRelationships = 27
            }
        };

    private static List<IdentityPoolConfiguration> TestLoadIdentityPool =>
    [
        new()
        {
            Type = "never",
            Name = "NeverUse",
            Alias = "e",
            Amount = 5,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
            NumberOfDatawalletModifications = 50,
            NumberOfDevices = 1,
            NumberOfChallenges = 0
        },
        new()
        {
            Type = "app",
            Name = "AppLight",
            Alias = "a1",
            Amount = 1,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 1,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
            NumberOfDatawalletModifications = 50,
            NumberOfDevices = 1,
            NumberOfChallenges = 1
        },
        new()
        {
            Type = "app",
            Name = "AppMedium",
            Alias = "a2",
            Amount = 3,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 2,
            NumberOfSentMessages = 4,
            NumberOfReceivedMessages = 4,
            NumberOfDatawalletModifications = 60,
            NumberOfDevices = 2,
            NumberOfChallenges = 10
        },
        new()
        {
            Type = "app",
            Name = "AppHeavy",
            Alias = "a3",
            Amount = 3,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 5,
            NumberOfSentMessages = 12,
            NumberOfReceivedMessages = 11,
            NumberOfDatawalletModifications = 1,
            NumberOfDevices = 3,
            NumberOfChallenges = 20
        },
        new()
        {
            Type = "connector",
            Name = "ConnectorLight",
            Alias = "c1",
            Amount = 1,
            NumberOfRelationshipTemplates = 1,
            NumberOfRelationships = 1,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 0
        },
        new()
        {
            Type = "connector",
            Name = "ConnectorMedium",
            Alias = "c2",
            Amount = 2,
            NumberOfRelationshipTemplates = 20,
            NumberOfRelationships = 3,
            NumberOfSentMessages = 8,
            NumberOfReceivedMessages = 5,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 20
        },
        new()
        {
            Type = "connector",
            Name = "ConnectorHeavy",
            Alias = "c3",
            Amount = 4,
            NumberOfRelationshipTemplates = 30,
            NumberOfRelationships = 5,
            NumberOfSentMessages = 8,
            NumberOfReceivedMessages = 9,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 30
        }
    ];

    private static VerificationConfiguration LightLoadVerificationConfiguration =>
        new()
        {
            App = new AppVerificationConfiguration
            {
                TotalNumberOfSentMessages = 16500,
                TotalNumberOfReceivedMessages = 16500,
                NumberOfReceivedMessagesAddOn = 0,
                TotalNumberOfRelationships = 1800
            },
            Connector = new ConnectorVerificationConfiguration
            {
                TotalNumberOfSentMessages = 16500,
                TotalNumberOfReceivedMessages = 16499,
                NumberOfReceivedMessagesAddOn = 1,
                TotalNumberOfAvailableRelationships = 1803
            }
        };

    private static List<IdentityPoolConfiguration> LightLoadIdentityPool =>
    [
        new()
        {
            Type = POOL_TYPE_NEVER,
            Name = POOL_NAME_NEVER,
            Alias = POOL_ALIAS_NEVER,
            Amount = 10,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
            NumberOfDatawalletModifications = 50,
            NumberOfDevices = 1,
            NumberOfChallenges = 0
        },
        new()
        {
            Type = POOL_TYPE_APP,
            Name = POOL_NAME_APP_LIGHT,
            Alias = POOL_ALIAS_APP_LIGHT,
            Amount = 50,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
            NumberOfDatawalletModifications = 5,
            NumberOfDevices = 1,
            NumberOfChallenges = 1
        },
        new()
        {
            Type = POOL_TYPE_APP,
            Name = POOL_NAME_APP_MEDIUM,
            Alias = POOL_ALIAS_APP_MEDIUM,
            Amount = 150,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 2,
            NumberOfSentMessages = 10,
            NumberOfReceivedMessages = 30,
            NumberOfDatawalletModifications = 60,
            NumberOfDevices = 2,
            NumberOfChallenges = 10
        },
        new()
        {
            Type = POOL_TYPE_APP,
            Name = POOL_NAME_APP_HEAVY,
            Alias = POOL_ALIAS_APP_HEAVY,
            Amount = 300,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 5,
            NumberOfSentMessages = 50,
            NumberOfReceivedMessages = 40,
            NumberOfDatawalletModifications = 1,
            NumberOfDevices = 3,
            NumberOfChallenges = 20
        },
        new()
        {
            Type = POOL_TYPE_CONNECTOR,
            Name = POOL_NAME_CONNECTOR_LIGHT,
            Alias = POOL_ALIAS_CONNECTOR_LIGHT,
            Amount = 2,
            NumberOfRelationshipTemplates = 1,
            NumberOfRelationships = 9,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 82,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 0
        },
        new()
        {
            Type = POOL_TYPE_CONNECTOR,
            Name = POOL_NAME_CONNECTOR_MEDIUM,
            Alias = POOL_ALIAS_CONNECTOR_MEDIUM,
            Amount = 15,
            NumberOfRelationshipTemplates = 20,
            NumberOfRelationships = 29,
            NumberOfSentMessages = 440,
            NumberOfReceivedMessages = 264,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 20
        },
        new()
        {
            Type = POOL_TYPE_CONNECTOR,
            Name = POOL_NAME_CONNECTOR_HEAVY,
            Alias = POOL_ALIAS_CONNECTOR_HEAVY,
            Amount = 15,
            NumberOfRelationshipTemplates = 30,
            NumberOfRelationships = 90,
            NumberOfSentMessages = 660,
            NumberOfReceivedMessages = 825,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 30
        }
    ];

    private static VerificationConfiguration HeavyLoadVerificationConfiguration =>
        new()
        {
            App = new AppVerificationConfiguration
            {
                TotalNumberOfSentMessages = 780000,
                TotalNumberOfReceivedMessages = 780000,
                NumberOfReceivedMessagesAddOn = 1000,
                TotalNumberOfRelationships = 21000
            },
            Connector = new ConnectorVerificationConfiguration
            {
                TotalNumberOfSentMessages = 781000,
                TotalNumberOfReceivedMessages = 780000,
                NumberOfReceivedMessagesAddOn = 0,
                TotalNumberOfAvailableRelationships = 21000
            }
        };

    private static List<IdentityPoolConfiguration> HeavyLoadIdentityPool =>
    [
        new()
        {
            Type = POOL_TYPE_NEVER,
            Name = POOL_NAME_NEVER,
            Alias = POOL_ALIAS_NEVER,
            Amount = 5000,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
            NumberOfDatawalletModifications = 50,
            NumberOfDevices = 1,
            NumberOfChallenges = 0
        },
        new()
        {
            Type = POOL_TYPE_APP,
            Name = POOL_NAME_APP_LIGHT,
            Alias = POOL_ALIAS_APP_LIGHT,
            Amount = 500,
            NumberOfRelationshipTemplates = 1,
            NumberOfRelationships = 1,
            NumberOfSentMessages = 10,
            NumberOfReceivedMessages = 15,
            NumberOfDatawalletModifications = 50,
            NumberOfDevices = 1,
            NumberOfChallenges = 1
        },
        new()
        {
            Type = POOL_TYPE_APP,
            Name = POOL_NAME_APP_MEDIUM,
            Alias = POOL_ALIAS_APP_MEDIUM,
            Amount = 1500,
            NumberOfRelationshipTemplates = 2,
            NumberOfRelationships = 2,
            NumberOfSentMessages = 50,
            NumberOfReceivedMessages = 130,
            NumberOfDatawalletModifications = 500,
            NumberOfDevices = 2,
            NumberOfChallenges = 10
        },
        new()
        {
            Type = POOL_TYPE_APP,
            Name = POOL_NAME_APP_HEAVY,
            Alias = POOL_ALIAS_APP_HEAVY,
            Amount = 3500,
            NumberOfRelationshipTemplates = 5,
            NumberOfRelationships = 5,
            NumberOfSentMessages = 200,
            NumberOfReceivedMessages = 165,
            NumberOfDatawalletModifications = 1500,
            NumberOfDevices = 3,
            NumberOfChallenges = 20
        },
        new()
        {
            Type = POOL_TYPE_CONNECTOR,
            Name = POOL_NAME_CONNECTOR_LIGHT,
            Alias = POOL_ALIAS_CONNECTOR_LIGHT,
            Amount = 10,
            NumberOfRelationshipTemplates = 100,
            NumberOfRelationships = 21,
            NumberOfSentMessages = 100,
            NumberOfReceivedMessages = 780,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 10
        },
        new()
        {
            Type = POOL_TYPE_CONNECTOR,
            Name = POOL_NAME_CONNECTOR_MEDIUM,
            Alias = POOL_ALIAS_CONNECTOR_MEDIUM,
            Amount = 20,
            NumberOfRelationshipTemplates = 8000,
            NumberOfRelationships = 252,
            NumberOfSentMessages = 12000,
            NumberOfReceivedMessages = 9360,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 100
        },
        new()
        {
            Type = POOL_TYPE_CONNECTOR,
            Name = POOL_NAME_CONNECTOR_HEAVY,
            Alias = POOL_ALIAS_CONNECTOR_HEAVY,
            Amount = 30,
            NumberOfRelationshipTemplates = 12000,
            NumberOfRelationships = 525,
            NumberOfSentMessages = 18000,
            NumberOfReceivedMessages = 19500,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 300
        }
    ];

    #endregion

    #region Deserialize From Json Tests

    [Theory]
    [InlineData("pool-config.heavy.json", "heavy")]
    [InlineData("pool-config.light.json", "light")]
    [InlineData("pool-config.test.json", "test")]
    public async Task DeserializeFromJson_InputPoolConfigJson_ReturnsPoolConfiguration(string poolConfigJsonFilename, string loadTestTag)
    {
        // Arrange
        var poolConfigJsonFile = Path.Combine(_testDataFolder, poolConfigJsonFilename);
        var expectedPoolConfig = GetExpectedPoolConfiguration(loadTestTag);

        // Act
        var actualPoolConfig = await IdentityPoolConfigGenerator.DeserializeFromJson(poolConfigJsonFile);

        // Assert
        actualPoolConfig.Should().NotBeNull();

        var areEqual = actualPoolConfig.Equals(expectedPoolConfig); //Note: Should().BeEquivalentTo does not invoke overridden Equals method
        areEqual.Should().BeTrue();
    }

    #endregion

    #region Deserialize From Excel Tests

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy")]
    [InlineData("PerformanceTestData.xlsx", "light")]
    [InlineData("PerformanceTestData.xlsx", "test")]
    public async Task DeserializeFromExcel_InputExcelFile_ReturnsPoolConfiguration(string excelFileName, string workSheetName)
    {
        // Arrange
        var excelFile = Path.Combine(_testDataFolder, excelFileName);
        var expectedPoolConfig = GetExpectedPoolConfiguration(loadTestTag: workSheetName);

        // Act
        var actualPoolConfig = await IdentityPoolConfigGenerator.DeserializeFromExcel(excelFile, workSheetName);

        // Assert
        actualPoolConfig.Should().NotBeNull();

        var areEqual = actualPoolConfig.Equals(expectedPoolConfig); //Note: Should().BeEquivalentTo does not invoke overridden Equals method
        areEqual.Should().BeTrue();
    }

    #endregion

    #region Verify Json Pool Config

    [Theory]
    [InlineData("PerformanceTestData.xlsx", "heavy", "pool-config.heavy.json")]
    [InlineData("PerformanceTestData.xlsx", "light", "pool-config.light.json")]
    [InlineData("PerformanceTestData.xlsx", "test", "pool-config.test.json")]
    public async Task VerifyJsonPoolConfig_InputPerformanceTestDataExcel_ReturnsSuccess(string excelFile, string workSheet, string expectedLoadTestJsonFilename)
    {
        // Arrange
        var expectedJson = Path.Combine(_testDataFolder, expectedLoadTestJsonFilename);
        var inputFile = Path.Combine(_testDataFolder, excelFile);

        var sut = new IdentityPoolConfigGenerator();

        // Act
        var result = await sut.VerifyJsonPoolConfig(inputFile, workSheet, expectedJson);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Generate Json Pool Config

    [Theory]
    [InlineData("PerformanceTestData-plusNewMedium.xlsx", WORKBOOK_SHEET_MEDIUM_LOAD, "pool-config.medium.json")]
    public async Task GenerateJsonPoolConfig_InputPerformanceTestDataExcel_ReturnsSuccess(string excelFile, string workSheet, string expectedLoadTestJsonFilename)
    {
        // Arrange
        var expectedPoolConfigJsonFilePath = Path.Combine(_testDataFolder, expectedLoadTestJsonFilename);
        var inputFile = Path.Combine(_testDataFolder, excelFile);

        var sut = new IdentityPoolConfigGenerator();

        // Act
        var (status, message) = await sut.GenerateJsonPoolConfig(inputFile, workSheet);

        // Assert

        status.Should().BeTrue();
        message.Should().Be(expectedPoolConfigJsonFilePath);

        File.Exists(expectedPoolConfigJsonFilePath).Should().BeTrue();
        (await sut.VerifyJsonPoolConfig(inputFile, workSheet, expectedPoolConfigJsonFilePath)).Should().BeTrue();
    }

    #endregion

    #region Generate Excel RelationshipsAndMessages Pool Config

    [Theory]
    [InlineData("test", "pool-config.test.json", "ExpectedRelationshipsAndMessagePoolConfigs.test.json")]
    public async Task GenerateExcelRelationshipsAndMessagesPoolConfig_InputPerformanceTestDataExcel_ReturnsSuccess(string workSheet, string poolConfigJsonFilename, string expectedLoadTestJsonFile)
    {
        // Arrange
        var poolConfigJsonFilePath = Path.Combine(_testDataFolder, poolConfigJsonFilename);
        var expectedRelationshipsFilePath = Path.Combine(_testDataFolder, $"{RELATIONSHIPS_AND_MESSAGE_POOL_CONFIGS_FILE_NAME}.{workSheet}.{EXCEL_FILE_EXT}");
        var expectedLoadTestDataFilePath = Path.Combine(_testDataFolder, expectedLoadTestJsonFile);

        await using var fileStream = File.OpenRead(expectedLoadTestDataFilePath);
        var expectedLoadTestData = await System.Text.Json.JsonSerializer.DeserializeAsync<RelationshipAndMessages[]>(fileStream);

        var sut = new IdentityPoolConfigGenerator();

        // Act
        var (status, message) = await sut.GenerateExcelRelationshipsAndMessagesPoolConfig(poolConfigJsonFilePath, workSheet);

        // Assert

        status.Should().BeTrue();
        message.Should().Be(expectedRelationshipsFilePath);

        File.Exists(expectedRelationshipsFilePath).Should().BeTrue();

        var actualLoadTestData = await (new ExcelMapper().FetchAsync<RelationshipAndMessages>(expectedRelationshipsFilePath));
        actualLoadTestData.Should().BeEquivalentTo(expectedLoadTestData);
    }

    #endregion
}
