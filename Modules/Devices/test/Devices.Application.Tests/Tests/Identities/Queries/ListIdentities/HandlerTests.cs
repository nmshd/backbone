﻿using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Handler = Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities.Handler;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class HandlerTests
{
    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async void Returns_an_empty_list_when_no_identities_exist()
    {
        // Arrange
        var identitiesList = new List<Identity>();
        var request = new PaginationFilter() { PageSize = 5 };
        var handler = CreateHandler(new FindAllStubRepository(MakeDBPaginationResult(identitiesList)));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(0);
    }

    [Fact]
    public async void Returns_a_list_of_all_existing_identities()
    {
        // Arrange
        var request = new PaginationFilter();
        List<Identity> identitiesList = new()
        {
            new(TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            TestDataGenerator.CreateRandomTierId(),
            1),
            new(TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            TestDataGenerator.CreateRandomTierId(),
            1)
        };

        var handler = CreateHandler(new FindAllStubRepository(MakeDBPaginationResult(identitiesList)));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async void Returned_identities_have_all_properties_filled_as_expected()
    {
        // Arrange
        var request = new PaginationFilter();
        var expectedClientId = TestDataGenerator.CreateRandomDeviceId();
        var expectedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var expectedTierId = TestDataGenerator.CreateRandomTierId();
        List<Identity> identitiesList = new()
        {
            new(expectedClientId, expectedAddress, Array.Empty<byte>(), expectedTierId, 1)
        };

        var handler = CreateHandler(new FindAllStubRepository(MakeDBPaginationResult(identitiesList)));

        // Act
        var result = await handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().ClientId.Should().Be(expectedClientId);
        result.First().Address.Should().Be(expectedAddress);
        result.First().PublicKey.Should().BeEquivalentTo(Array.Empty<byte>());
        result.First().TierId.Should().BeEquivalentTo(expectedTierId);
        result.First().IdentityVersion.Should().Be(1);
    }

    private Handler CreateHandler(FindAllStubRepository findAllStubRepository)
    {
        return new Handler(findAllStubRepository);
    }

    private DbPaginationResult<Identity> MakeDBPaginationResult(List<Identity> identities)
    {
        return new DbPaginationResult<Identity>(identities, identities.Count);
    }
}
