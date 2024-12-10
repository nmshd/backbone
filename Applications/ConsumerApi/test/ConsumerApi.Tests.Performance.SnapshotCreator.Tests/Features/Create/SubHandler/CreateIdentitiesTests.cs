using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateIdentitiesTests : SnapshotCreatorTestsBase
{
    private readonly CreateIdentities.CommandHandler _sut;
    private readonly IIdentityFactory _identityFactory;

    public CreateIdentitiesTests()
    {
        _identityFactory = A.Fake<IIdentityFactory>();
        _sut = new CreateIdentities.CommandHandler(_identityFactory);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfDomainIdentities_WhenValidCommand()
    {
        // Arrange
        var identities = new List<IdentityPoolConfiguration>()
        {
            new(new PoolConfiguration() { Alias = "e", Amount = 1, Type = POOL_TYPE_NEVER }),
            new(new PoolConfiguration() { Alias = "a1", Amount = 1, Type = POOL_TYPE_APP }),
            new(new PoolConfiguration() { Alias = "c1", Amount = 1, Type = POOL_TYPE_CONNECTOR })
        };

        var fakeDomainIdentity = A.Fake<DomainIdentity>();

        A.CallTo(() => _identityFactory.Create(
            A<CreateIdentities.Command>.Ignored,
            A<IdentityConfiguration>.Ignored)).Returns(fakeDomainIdentity);

        var command = new CreateIdentities.Command(
            identities,
            "http://localhost:8081",
            new ClientCredentials("test", "test")
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        A.CallTo(() => _identityFactory.Create(
            A<CreateIdentities.Command>.Ignored,
            A<IdentityConfiguration>.Ignored)).MustHaveHappened(identities.Count, Times.Exactly);

        result.Count.Should().Be(identities.Count);
        _identityFactory.TotalConfiguredIdentities.Should().Be(result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoIdentitiesProvided()
    {
        // ARRANGE
        var command = new CreateIdentities.Command(
            [],
            "http://localhost:8081",
            new ClientCredentials("test", "test")
        );

        // ACT
        var result = await _sut.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        A.CallTo(() => _identityFactory.Create(
            A<CreateIdentities.Command>.Ignored,
            A<IdentityConfiguration>.Ignored)).MustNotHaveHappened();
        _identityFactory.TotalConfiguredIdentities.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenNullIdentitiesProvided()
    {
        // ARRANGE
        var command = new CreateIdentities.Command(
            null!,
            "http://localhost:8081",
            new ClientCredentials("test", "test")
        );

        // ACT & ASSERT
        var act = () => _sut.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
