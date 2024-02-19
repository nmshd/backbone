using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetIdentity;
public class HandlerTests
{
    [Fact]
    public async void Gets_identity_by_address()
    {
        // Arrange
        var identity = new Identity(CreateRandomDeviceId(), CreateRandomIdentityAddress(), [1, 1, 1, 1, 1], TestDataGenerator.CreateRandomTierId(), 1);

        var handler = CreateHandler(new FindByAddressStubRepository(identity));

        // Act
        var result = await handler.Handle(new GetIdentityQuery(identity.Address), CancellationToken.None);

        // Assert
        result.ClientId.Should().Be(identity.ClientId);
        result.Address.Should().Be(identity.Address);
        result.PublicKey.Should().BeEquivalentTo(identity.PublicKey);
        result.TierId.Should().BeEquivalentTo(identity.TierId!);
        result.IdentityVersion.Should().Be(1);
    }

    [Fact]
    public void Fails_when_no_identity_found()
    {
        // Arrange
        var identityRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identityRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(identityRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(new GetIdentityQuery("some-inexistent-identity-address"), CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
