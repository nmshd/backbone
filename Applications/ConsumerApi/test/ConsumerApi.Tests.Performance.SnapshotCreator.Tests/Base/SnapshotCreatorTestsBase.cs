using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.UnitTestTools.BaseClasses;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;

public abstract class SnapshotCreatorTestsBase : AbstractTestsBase
{
    protected string TestDataFolder { get; } = Path.Combine(AppContext.BaseDirectory, "TestData");

    #region Helper Methods

    protected virtual PerformanceTestConfiguration GetExpectedPoolConfiguration(string loadTestTag)
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

    protected virtual VerificationConfiguration TestLoadVerificationConfiguration =>
        new()
        {
            TotalNumberOfRelationships = 22,
            TotalAppSentMessages = 48,
            TotalConnectorSentMessages = 48
        };

    protected virtual List<IdentityPoolConfiguration> TestLoadIdentityPool =>
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

    protected virtual VerificationConfiguration LightLoadVerificationConfiguration =>
        new()
        {
            TotalNumberOfRelationships = 1800,
            TotalAppSentMessages = 16500,
            TotalConnectorSentMessages = 16500
        };

    protected virtual List<IdentityPoolConfiguration> LightLoadIdentityPool =>
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
            NumberOfReceivedMessages = 41,
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

    protected virtual VerificationConfiguration HeavyLoadVerificationConfiguration =>
        new()
        {
            TotalNumberOfRelationships = 20500,
            TotalAppSentMessages = 775000,
            TotalConnectorSentMessages = 780000
        };

    protected virtual List<IdentityPoolConfiguration> HeavyLoadIdentityPool =>
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
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
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
            NumberOfReceivedMessages = 167,
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
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 0,
            NumberOfReceivedMessages = 0,
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
            NumberOfRelationships = 257,
            NumberOfSentMessages = 12000,
            NumberOfReceivedMessages = 9687,
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
            NumberOfRelationships = 513,
            NumberOfSentMessages = 18000,
            NumberOfReceivedMessages = 19375,
            NumberOfDatawalletModifications = 0,
            NumberOfDevices = 1,
            NumberOfChallenges = 300
        }
    ];

    #endregion
}
