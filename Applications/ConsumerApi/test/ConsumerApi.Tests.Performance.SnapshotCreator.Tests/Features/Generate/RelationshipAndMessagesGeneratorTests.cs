using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Generate;

public class RelationshipAndMessagesGeneratorTests : SnapshotCreatorTestsBase
{
    [Theory]
    [InlineData("expected-pool-config.test.json")]
    [InlineData("expected-pool-config.light.json")]
    [InlineData("expected-pool-config.heavy.json")]
    public async Task Generate_InputPerformanceTestData_ReturnsSuccess(string expectedLoadTestJsonFile)
    {
        // Arrange
        var poolConfiguration = await GetExpectedPoolConfiguration(expectedLoadTestJsonFile);

        var sut = new RelationshipAndMessagesGenerator();

        // Act
        var result = sut.Generate(poolConfiguration!);

        // Assert
        result.Should().NotBeEmpty();
        var allAppMessages = result.Where(r => r.SenderIdentityPoolType == IdentityPoolType.App).Sum(x => x.NumberOfSentMessages);
        allAppMessages.Should().Be(poolConfiguration!.VerificationConfiguration.TotalAppSentMessages);

        var allConnectorMessages = result.Where(r => r.SenderIdentityPoolType == IdentityPoolType.Connector).Sum(x => x.NumberOfSentMessages);
        allConnectorMessages.Should().Be(poolConfiguration!.VerificationConfiguration.TotalConnectorSentMessages);

        var allRelationships = result.Length / 2;
        allRelationships.Should().Be(poolConfiguration!.VerificationConfiguration.TotalNumberOfRelationships);
    }

    [Theory]
    [InlineData("app")]
    [InlineData("connector")]
    [InlineData("never")]
    public void VerifyNumberOfSentMessages_ExpectedTotalNumberOfSentMessagesIsTrue_Succeeds(string poolTypeString)
    {
        // Arrange
        var poolType = poolTypeString switch
        {
            "app" => IdentityPoolType.App,
            "connector" => IdentityPoolType.Connector,
            "never" => IdentityPoolType.Never,
            _ => throw new InvalidOperationException("Unknown pool type")
        };

        var relationshipAndMessages = new RelationshipAndMessages[]
        {
            new("app", 1, "connector", 1) { NumberOfSentMessages = 1 },
            new("app", 2, "connector", 2) { NumberOfSentMessages = 2 },
            new("app", 3, "connector", 3) { NumberOfSentMessages = 3 },
            new("app", 4, "connector", 4) { NumberOfSentMessages = 4 },
            new("connector", 4, "app", 4) { NumberOfSentMessages = 5 },
            new("connector", 3, "app", 3) { NumberOfSentMessages = 6 },
            new("connector", 2, "app", 2) { NumberOfSentMessages = 7 },
            new("connector", 1, "app", 1) { NumberOfSentMessages = 8 }
        };

        var expectedTotalNumberOfSentMessages = relationshipAndMessages
            .Where(r => r.RecipientIdentityPoolType == poolType)
            .Sum(x => x.NumberOfSentMessages);

        // Act
        var act = () => RelationshipAndMessagesGenerator.VerifyNumberOfSentMessages(relationshipAndMessages, poolType, expectedTotalNumberOfSentMessages);

        // Assert
        act.Should().NotThrow<InvalidOperationException>();
    }

    [Theory]
    [InlineData("app")]
    [InlineData("connector")]
    [InlineData("never")]
    public void VerifyNumberOfSentMessages_ExpectedTotalNumberOfSentMessagesIsFalse_Succeeds(string poolTypeString)
    {
        // Arrange
        var poolType = poolTypeString switch
        {
            "app" => IdentityPoolType.App,
            "connector" => IdentityPoolType.Connector,
            "never" => IdentityPoolType.Never,
            _ => throw new InvalidOperationException("Unknown pool type")
        };

        var relationshipAndMessages = new RelationshipAndMessages[]
        {
            new("app", 1, "connector", 1) { NumberOfSentMessages = 1 },
            new("app", 2, "connector", 2) { NumberOfSentMessages = 2 },
            new("app", 3, "connector", 3) { NumberOfSentMessages = 3 },
            new("app", 4, "connector", 4) { NumberOfSentMessages = 4 },
            new("connector", 4, "app", 4) { NumberOfSentMessages = 5 },
            new("connector", 3, "app", 3) { NumberOfSentMessages = 6 },
            new("connector", 2, "app", 2) { NumberOfSentMessages = 7 },
            new("connector", 1, "app", 1) { NumberOfSentMessages = 8 }
        };

        var expectedTotalNumberOfSentMessages = relationshipAndMessages
            .Where(r => r.RecipientIdentityPoolType == poolType)
            .Sum(x => x.NumberOfSentMessages) + 1;

        // Act
        var act = () => RelationshipAndMessagesGenerator.VerifyNumberOfSentMessages(relationshipAndMessages, poolType, expectedTotalNumberOfSentMessages);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }


    [Fact]
    public void TryFindAnotherConnectorIdentityForThatAppIdentity_WhenNoOtherConnectorIdentityAvailable_ShouldThrowException()
    {
        // Arrange
        var appIdentity = new IdentityConfiguration(1, IdentityPoolType.App, new PoolConfiguration() { Alias = "app", NumberOfRelationships = 1 });
        var appIdentities = new List<IdentityConfiguration>() { appIdentity };

        var connectorIdentity = new IdentityConfiguration(1, IdentityPoolType.Connector, new PoolConfiguration() { Alias = "connector", NumberOfRelationships = 1 });
        connectorIdentity.RelationshipAndMessages.Add(new RelationshipAndMessages(connectorIdentity.PoolAlias, connectorIdentity.Address, appIdentity.PoolAlias, appIdentity.Address));

        var connectorIdentities = new List<IdentityConfiguration> { connectorIdentity };
        IdentityConfiguration? recipientConnectorIdentity = null;

        // Act
        var act = () => RelationshipAndMessagesGenerator.TryFindAnotherConnectorIdentityForThatAppIdentity(
            connectorIdentities,
            appIdentity,
            recipientConnectorIdentity,
            appIdentities);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void TryFindConnectorIdentityForThatAppIdentity_WhenConnectorIdentityHasNoAvailableRelationships_ShouldContinueAndReturnNull()
    {
        // Arrange
        var appIdentity = new IdentityConfiguration(1, IdentityPoolType.App, new PoolConfiguration() { Alias = "app", NumberOfRelationships = 1 });
        var connectorIdentity = new IdentityConfiguration(1, IdentityPoolType.Connector, new PoolConfiguration() { Alias = "connector", NumberOfRelationships = 1 })
        {
            NumberOfRelationships = 0
        };
        var connectorIdentities = new List<IdentityConfiguration> { connectorIdentity };

        // Act
        var result = RelationshipAndMessagesGenerator.TryFindConnectorIdentityForThatAppIdentity(connectorIdentities, appIdentity);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void TryFindConnectorIdentityForThatAppIdentity_WhenAppHasRelationship_ShouldContinueAndReturnNull()
    {
        // Arrange
        var appIdentity = new IdentityConfiguration(1, IdentityPoolType.App, new PoolConfiguration() { Alias = "app", NumberOfRelationships = 1 });
        var connectorIdentity = new IdentityConfiguration(1, IdentityPoolType.Connector, new PoolConfiguration() { Alias = "connector", NumberOfRelationships = 1 });
        appIdentity.RelationshipAndMessages.Add(new RelationshipAndMessages(appIdentity.PoolAlias, appIdentity.Address, connectorIdentity.PoolAlias, connectorIdentity.Address));

        var connectorIdentities = new List<IdentityConfiguration> { connectorIdentity };

        // Act
        var result = RelationshipAndMessagesGenerator.TryFindConnectorIdentityForThatAppIdentity(connectorIdentities, appIdentity);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void TryFindConnectorIdentityForThatAppIdentity_WhenAppHasNoRelationship_ShouldContinueAndReturnConnectorIdentity()
    {
        // Arrange
        var appIdentity = new IdentityConfiguration(1, IdentityPoolType.App, new PoolConfiguration() { Alias = "app", NumberOfRelationships = 1 });
        var connectorIdentity = new IdentityConfiguration(1, IdentityPoolType.Connector, new PoolConfiguration() { Alias = "connector", NumberOfRelationships = 1 });
        var connectorIdentities = new List<IdentityConfiguration> { connectorIdentity };

        // Act
        var result = RelationshipAndMessagesGenerator.TryFindConnectorIdentityForThatAppIdentity(connectorIdentities, appIdentity);

        // Assert
        result.Should().BeSameAs(connectorIdentity);
    }
}
