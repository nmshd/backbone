using Backbone.BuildingBlocks.Application.Pagination;

namespace Backbone.BuildingBlocks.Application.Tests.Pagination;

public class PaginationDataTests : AbstractTestsBase
{
    [Theory]
    [InlineData(10, 1, 10)]
    [InlineData(10, 2, 5)]
    [InlineData(10, 10, 1)]
    [InlineData(10, 50, 1)]
    public void TotalPages_is_calculated_correctly(int totalRecords, int pageSize, int expectedTotalPages)
    {
        var sut = CreatePaginationData(1, pageSize, totalRecords);

        sut.TotalPages.ShouldBe(expectedTotalPages);
    }

    [Fact]
    public void Has_single_page_when_PageSize_is_not_provided()
    {
        const int totalRecords = 49;

        var sut = CreatePaginationData(1, null, totalRecords);

        sut.PageSize.ShouldBe(totalRecords);
    }

    private static PaginationData CreatePaginationData(int pageNumber, int? pageSize, int totalRecords)
    {
        return new PaginationData(new PaginationFilter(pageNumber, pageSize), totalRecords);
    }
}
