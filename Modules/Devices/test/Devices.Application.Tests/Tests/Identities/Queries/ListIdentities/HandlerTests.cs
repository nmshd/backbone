using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Returns_an_empty_list_when_no_identities_exist()
    {
        // Arrange
        var identities = new List<Identity>();
        var handler = CreateHandler(new FindAllStubRepository(identities));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identities.Count);
    }

    [Fact]
    public async Task? Returns_a_list_of_all_existing_identities()
    {
        // Arrange
        List<Identity> identities =
        [
            new Identity(CreateRandomDeviceId(),
                CreateRandomIdentityAddress(),
                CreateRandomBytes(),
                TestDataGenerator.CreateRandomTierId(),
                1),

            new Identity(CreateRandomDeviceId(),
                CreateRandomIdentityAddress(),
                CreateRandomBytes(),
                TestDataGenerator.CreateRandomTierId(),
                1)
        ];
        var handler = CreateHandler(new FindAllStubRepository(identities));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identities.Count);
    }

    [Fact]
    public async Task Returned_identities_have_all_properties_filled_as_expected()
    {
        // Arrange
        var expectedClientId = CreateRandomDeviceId();
        var expectedAddress = CreateRandomIdentityAddress();
        var expectedTierId = TestDataGenerator.CreateRandomTierId();
        List<Identity> identities = [new Identity(expectedClientId, expectedAddress, [], expectedTierId, 1)];

        var handler = CreateHandler(new FindAllStubRepository(identities));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().ClientId.Should().Be(expectedClientId);
        result.First().Address.Should().Be(expectedAddress);
        result.First().PublicKey.Should().BeEquivalentTo(Array.Empty<byte>());
        result.First().TierId.Should().BeEquivalentTo(expectedTierId);
        result.First().Status.Should().Be(IdentityStatus.Active);
        result.First().IdentityVersion.Should().Be(1);
    }

    private Handler CreateHandler(FindAllStubRepository findAllStubRepository)
    {
        return new Handler(findAllStubRepository);
    }
}
