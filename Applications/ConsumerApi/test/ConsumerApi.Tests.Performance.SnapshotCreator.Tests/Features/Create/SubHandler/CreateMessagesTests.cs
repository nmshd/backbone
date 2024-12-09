using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateMessagesTests
{
    [Fact]
    public async Task Handle_ShouldCreateMessages()
    {
        // Arrange
        var messageFactory = A.Fake<IMessageFactory>();
        var sut = new CreateMessages.CommandHandler(messageFactory);

        var appIdentity = new DomainIdentity(null!, null, 1, 0, 2, IdentityPoolType.App, 5, "a1", 2, 10);
        var connectorIdentity = new DomainIdentity(null!, null, 1, 0, 3, IdentityPoolType.Connector, 5, "c1", 3, 20);
        var neverUsedIdentity = new DomainIdentity(null!, null, 1, 0, 0, IdentityPoolType.Never, 5, "e", 5, 5);
        var identities = new List<DomainIdentity>
        {
            appIdentity,
            neverUsedIdentity,
            connectorIdentity
        };

        var relationshipAndMessages = new List<RelationshipAndMessages>
        {
            new(appIdentity.PoolAlias, appIdentity.ConfigurationIdentityAddress, connectorIdentity.PoolAlias, connectorIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = appIdentity.NumberOfSentMessages
            },
            new(connectorIdentity.PoolAlias, connectorIdentity.ConfigurationIdentityAddress, appIdentity.PoolAlias, appIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = connectorIdentity.NumberOfSentMessages
            }
        };

        var expectedTotalMessages = identities.Where(i => i.IdentityPoolType != IdentityPoolType.Never).Sum(i => i.NumberOfSentMessages);

        var request = new CreateMessages.Command(identities, relationshipAndMessages, "http://baseurl", new ClientCredentials("clientId", "clientSecret"));

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => messageFactory.Create(A<CreateMessages.Command>.Ignored, A<DomainIdentity>.Ignored))
            .MustHaveHappened(relationshipAndMessages.Count, Times.Exactly);

        messageFactory.TotalMessages.Should().Be(expectedTotalMessages);
    }

    [Fact]
    public async Task Handle_RelationshipAndMessagesIsEmpty_ShouldDoNothing()
    {
        // Arrange
        var messageFactory = A.Fake<IMessageFactory>();
        var sut = new CreateMessages.CommandHandler(messageFactory);

        var appIdentity = new DomainIdentity(null!, null, 1, 0, 2, IdentityPoolType.App, 5, "a1", 2, 10);
        var connectorIdentity = new DomainIdentity(null!, null, 1, 0, 3, IdentityPoolType.Connector, 5, "c1", 3, 20);
        var neverUsedIdentity = new DomainIdentity(null!, null, 1, 0, 0, IdentityPoolType.Never, 5, "e", 5, 5);
        var identities = new List<DomainIdentity>
        {
            appIdentity,
            neverUsedIdentity,
            connectorIdentity
        };

        var relationshipAndMessages = new List<RelationshipAndMessages>();

        const int expectedTotalMessages = 0;

        var request = new CreateMessages.Command(identities, relationshipAndMessages, "http://baseurl", new ClientCredentials("clientId", "clientSecret"));

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => messageFactory.Create(A<CreateMessages.Command>.Ignored, A<DomainIdentity>.Ignored))
            .MustNotHaveHappened();

        messageFactory.TotalMessages.Should().Be(expectedTotalMessages);
    }

    [Fact]
    public async Task Handle_InvalidMessageConfiguration_ShouldThrowException()
    {
        // Arrange
        var messageFactory = A.Fake<IMessageFactory>();
        var sut = new CreateMessages.CommandHandler(messageFactory);

        var appIdentity = new DomainIdentity(null!, null, 1, 0, 2, IdentityPoolType.App, 5, "a1", 2, 10);
        var connectorIdentity = new DomainIdentity(null!, null, 1, 0, 3, IdentityPoolType.Connector, 5, "c1", 3, 20);
        var neverUsedIdentity = new DomainIdentity(null!, null, 1, 0, 0, IdentityPoolType.Never, 5, "e", 5, 5);
        var identities = new List<DomainIdentity>
        {
            appIdentity,
            neverUsedIdentity,
            connectorIdentity
        };

        const int digit = 1;
        var relationshipAndMessages = new List<RelationshipAndMessages>
        {
            new(appIdentity.PoolAlias, appIdentity.ConfigurationIdentityAddress, connectorIdentity.PoolAlias, connectorIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = appIdentity.NumberOfSentMessages
            },
            new(connectorIdentity.PoolAlias, connectorIdentity.ConfigurationIdentityAddress, appIdentity.PoolAlias, appIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = connectorIdentity.NumberOfSentMessages - digit
            }
        };

        var expectedTotalMessages = identities.Where(i => i.IdentityPoolType != IdentityPoolType.Never).Sum(i => i.NumberOfSentMessages);

        var request = new CreateMessages.Command(identities, relationshipAndMessages, "http://baseurl", new ClientCredentials("clientId", "clientSecret"));

        // Act
        var act = () => sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => messageFactory.Create(A<CreateMessages.Command>.Ignored, A<DomainIdentity>.Ignored))
            .MustNotHaveHappened();

        messageFactory.TotalMessages.Should().Be(expectedTotalMessages - digit);
    }
}
