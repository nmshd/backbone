namespace Backbone.BuildingBlocks.Application.Pagination;

public class PaginationFilter
{
    private const int DEFAULT_PAGE_NUMBER = 1;

    public PaginationFilter() : this(null, null)
    {
    }

    public PaginationFilter(int? pageNumber, int? pageSize)
    {
        PageNumber = !pageNumber.HasValue || pageNumber.Value < 1 ? DEFAULT_PAGE_NUMBER : pageNumber.Value;
        PageSize = pageSize;
    }

    public int PageNumber { get; set; }
    public int? PageSize { get; set; }
}
