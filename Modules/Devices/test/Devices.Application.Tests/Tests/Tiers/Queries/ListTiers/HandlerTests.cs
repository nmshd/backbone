using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Handler = Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers.Handler;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Queries.ListTiers;

public class HandlerTests
{
    private readonly ITierRepository _fakeRepository;
    private readonly Handler _handler;

    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        _fakeRepository = A.Fake<ITierRepository>();

        _handler = CreateHandler(); 
    }

    [Fact]
    public async void Returns_an_empty_list_when_no_tiers_exist()
    {
        // Arrange
        var tiersList = new List<Tier>();
        var request = new PaginationFilter() { PageSize = 5 };
        A.CallTo(() => _fakeRepository.FindAll(request)).Returns(MakeDBPaginationResult(tiersList));

        // Act
        var result = await _handler.Handle(new ListTiersQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(0);
    }

    [Fact]
    public async void Returns_a_list_of_all_existing_tiers()
    {
        // Arrange
        var request = new PaginationFilter();
        List<Tier> tiersList = new()
        {
            new(TierName.Create("my-tier-name-1").Value),
            new(TierName.Create("my-tier-name-2").Value)
        };

        A.CallTo(() => _fakeRepository.FindAll(request)).Returns(MakeDBPaginationResult(tiersList));

        // Act
        var result = await _handler.Handle(new ListTiersQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async void Returned_tiers_have_all_properties_filled_as_expected()
    {
        // Arrange
        var request = new PaginationFilter();
        var expectedName = TierName.Create("my-tier-name").Value;
        List<Tier> tiersList = new()
        {
            new(expectedName)
        };

        A.CallTo(() => _fakeRepository.FindAll(request)).Returns(MakeDBPaginationResult(tiersList));

        // Act
        var result = await _handler.Handle(new ListTiersQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be(expectedName);
    }

    private Handler CreateHandler()
    {
        return new Handler(_fakeRepository);
    }

    private DbPaginationResult<Tier> MakeDBPaginationResult(List<Tier> tiers)
    {
        return new DbPaginationResult<Tier>(tiers, tiers.Count);
    }
}
