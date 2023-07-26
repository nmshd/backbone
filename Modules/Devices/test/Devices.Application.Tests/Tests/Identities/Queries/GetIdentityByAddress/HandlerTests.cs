using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentityByAddress;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetIdentityByAddress;
public class HandlerTests
{
    [Fact]
    public async void Gets_identity_by_address()
    {
        // Arrange
        var expectedClientId = TestDataGenerator.CreateRandomDeviceId();
        var expectedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var expectedTierId = TestDataGenerator.CreateRandomTierId();
        var identity = new Identity(expectedClientId, expectedAddress, Array.Empty<byte>(), expectedTierId, 1);

        var handler = CreateHandler(new FindByAddressStubRepository(identity));

        // Act
        var result = await handler.Handle(new GetIdentityByAddressQuery(identity.Address), CancellationToken.None);

        // Assert
        result.ClientId.Should().Be(expectedClientId);
        result.Address.Should().Be(expectedAddress);
        result.PublicKey.Should().BeEquivalentTo(Array.Empty<byte>());
        result.TierId.Should().BeEquivalentTo(expectedTierId);
        result.IdentityVersion.Should().Be(1);
    }

    [Fact]
    public async void Fails_when_no_identity_found()
    {
        // Arrange
        var identityRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identityRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._)).Returns((Identity)null);

        var handler = CreateHandler(identityRepository);

        // Act
        var acting = async () => await handler.Handle(new GetIdentityByAddressQuery("some-inexistent-identity-address"), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
