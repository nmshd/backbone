using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateIdentitiesTests : SnapshotCreatorTestsBase
{
    [Fact]
    public async Task Handle_ShouldReturnListOfDomainIdentities_WhenValidCommand()
    {
        // Arrange
        var identityFactory = A.Fake<IIdentityFactory>();

        var identities = new List<IdentityPoolConfiguration>()
        {
            new(new PoolConfiguration() { Alias = "e", Amount = 1, Type = POOL_TYPE_NEVER }),
            new(new PoolConfiguration() { Alias = "a1", Amount = 1, Type = POOL_TYPE_APP }),
            new(new PoolConfiguration() { Alias = "c1", Amount = 1, Type = POOL_TYPE_CONNECTOR })
        };

        var fakeDomainIdentity = A.Fake<DomainIdentity>();

        A.CallTo(() => identityFactory.Create(
            A<CreateIdentities.Command>.Ignored,
            A<IdentityConfiguration>.Ignored)).Returns(fakeDomainIdentity);


        var handler = new CreateIdentities.CommandHandler(identityFactory);
        var command = new CreateIdentities.Command(
            identities,
            "http://localhost:8081",
            new ClientCredentials("test", "test")
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        A.CallTo(() => identityFactory.Create(
            A<CreateIdentities.Command>.Ignored,
            A<IdentityConfiguration>.Ignored)).MustHaveHappened(identities.Count, Times.Exactly);

        result.Count.Should().Be(identities.Count);
        identityFactory.TotalIdentities.Should().Be(result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoIdentitiesProvided()
    {
        var identityFactory = A.Fake<IIdentityFactory>();
        var handler = new CreateIdentities.CommandHandler(identityFactory);
        var command = new CreateIdentities.Command(
            new List<IdentityPoolConfiguration>(),
            "http://localhost:8081",
            new ClientCredentials("test", "test")
        );

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        A.CallTo(() => identityFactory.Create(
            A<CreateIdentities.Command>.Ignored,
            A<IdentityConfiguration>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenNullIdentitiesProvided()
    {
        var identityFactory = A.Fake<IIdentityFactory>();
        var handler = new CreateIdentities.CommandHandler(identityFactory);
        var command = new CreateIdentities.Command(
            null!,
            "http://localhost:8081",
            new ClientCredentials("test", "test")
        );

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, CancellationToken.None));
    }
}
