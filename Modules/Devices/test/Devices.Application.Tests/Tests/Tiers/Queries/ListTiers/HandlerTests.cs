using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Queries.ListTiers;

public class HandlerTests
{
    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async void Returns_an_empty_list_when_no_tiers_exist()
    {
        // Arrange
        var tiersList = new List<Tier>();
        var request = new PaginationFilter() { PageSize = 5 };
        var handler = CreateHandler(new FindAllStubRepository(MakeDbPaginationResult(tiersList)));

        // Act
        var result = await handler.Handle(new ListTiersQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(0);
    }

    [Fact]
    public async void Returns_a_list_of_all_existing_tiers()
    {
        // Arrange
        var request = new PaginationFilter();
        List<Tier> tiersList =
        [
            new Tier(TierName.Create("my-tier-name-1").Value),
            new Tier(TierName.Create("my-tier-name-2").Value)
        ];

        var handler = CreateHandler(new FindAllStubRepository(MakeDbPaginationResult(tiersList)));

        // Act
        var result = await handler.Handle(new ListTiersQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async void Returned_tiers_have_all_properties_filled_as_expected()
    {
        // Arrange
        var request = new PaginationFilter();
        var expectedName = TierName.Create("my-tier-name").Value;
        List<Tier> tiersList = [new Tier(expectedName)];

        var handler = CreateHandler(new FindAllStubRepository(MakeDbPaginationResult(tiersList)));

        // Act
        var result = await handler.Handle(new ListTiersQuery(request), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().NotBeNull();
        result.First().Name.Should().Be(expectedName);
    }

    private Handler CreateHandler(FindAllStubRepository findAllStubRepository)
    {
        return new Handler(findAllStubRepository);
    }

    private DbPaginationResult<Tier> MakeDbPaginationResult(List<Tier> tiers)
    {
        return new DbPaginationResult<Tier>(tiers, tiers.Count);
    }
}
