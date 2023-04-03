using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Handler = Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities.Handler;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

using FakeItEasy;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class HandlerTests
{
    private readonly IIdentityRepository _fakeRepository;
    private readonly Handler _handler;

    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        _fakeRepository = A.Fake<IIdentityRepository>();

        _handler = CreateHandler(); 
    }

    [Fact]
    public async void Returns_an_empty_list_when_no_identities_exist()
    {
        // Arrange
        var identitiesList = new List<Identity>();
        var request = new PaginationFilter() { PageSize = 5 };
        A.CallTo(() => _fakeRepository.FindAll(request)).Returns(MakeDBPaginationResult(identitiesList));

        // Act
        var result = await _handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(0);
    }

    [Fact]
    public async void Returns_list_when_identities_loaded()
    {
        // Arrange
        var request = new PaginationFilter();
        List<Identity> identitiesList = new()
        {
            new(TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            1),
            new(TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            1)
        };

        A.CallTo(() => _fakeRepository.FindAll(request)).Returns(MakeDBPaginationResult(identitiesList));

        // Act
        var result = await _handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async void Returns_list_when_identities_loaded_and_attributes_match()
    {
        // Arrange
        var request = new PaginationFilter();
        var expectedClientId = TestDataGenerator.CreateRandomDeviceId();
        var expectedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        List<Identity> identitiesList = new()
        {
            new(expectedClientId, expectedAddress, Array.Empty<byte>(), 1)
        };

        A.CallTo(() => _fakeRepository.FindAll(request)).Returns(MakeDBPaginationResult(identitiesList));

        // Act
        var result = await _handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().ClientId.Should().Be(expectedClientId);
        result.First().Address.Should().Be(expectedAddress);
        result.First().PublicKey.Should().BeEquivalentTo(Array.Empty<byte>());
        result.First().IdentityVersion.Should().Be(1);
    }

    private Handler CreateHandler()
    {
        return new Handler(_fakeRepository);
    }

    private DbPaginationResult<Identity> MakeDBPaginationResult(List<Identity> identities)
    {
        return new DbPaginationResult<Identity>(identities, identities.Count);
    }
}
