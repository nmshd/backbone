using Backbone.DevelopmentKit.Identity.ValueObjects;
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
        var identitiesList = new List<Identity>();
        var handler = CreateHandler(new FindAllFakeRepository(identitiesList));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(new List<IdentityAddress>()), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identitiesList.Count);
    }

    [Fact]
    public async Task Returns_a_list_of_all_identities_when_addresses_list_is_empty()
    {
        // Arrange
        List<Identity> identitiesList =
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
        var handler = CreateHandler(new FindAllFakeRepository(identitiesList));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(new List<IdentityAddress>()), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identitiesList.Count);
    }


    [Fact]
    public async Task Returns_a_list_of_identities_with_matching_address_from_addresses_list_when_not_empty()
    {
        // Arrange
        var firstIdentityAddress = CreateRandomIdentityAddress();
        List<Identity> identitiesList =
        [
            new Identity(CreateRandomDeviceId(),
                firstIdentityAddress,
                CreateRandomBytes(),
                TestDataGenerator.CreateRandomTierId(),
                1),

            new Identity(CreateRandomDeviceId(),
                CreateRandomIdentityAddress(),
                CreateRandomBytes(),
                TestDataGenerator.CreateRandomTierId(),
                1)
        ];
        var addresses = new List<IdentityAddress> { firstIdentityAddress };
        var handler = CreateHandler(new FindAllFakeRepository(identitiesList));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(addresses), CancellationToken.None);

        // Assert
        result.Should().HaveCount(addresses.Count);
    }

    [Fact]
    public async Task Returned_identities_have_all_properties_filled_as_expected()
    {
        // Arrange
        var expectedClientId = CreateRandomDeviceId();
        var expectedAddress = CreateRandomIdentityAddress();
        var expectedTierId = TestDataGenerator.CreateRandomTierId();
        List<Identity> identitiesList = [new Identity(expectedClientId, expectedAddress, [], expectedTierId, 1)];

        var handler = CreateHandler(new FindAllFakeRepository(identitiesList));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(new List<IdentityAddress>()), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().ClientId.Should().Be(expectedClientId);
        result.First().Address.Should().Be(expectedAddress);
        result.First().PublicKey.Should().BeEquivalentTo(Array.Empty<byte>());
        result.First().TierId.Should().BeEquivalentTo(expectedTierId);
        result.First().Status.Should().Be(IdentityStatus.Active);
        result.First().IdentityVersion.Should().Be(1);
    }

    private Handler CreateHandler(FindAllFakeRepository findAllFakeRepository)
    {
        return new Handler(findAllFakeRepository);
    }
}
