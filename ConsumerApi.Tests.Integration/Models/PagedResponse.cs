namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class PagedResponse<T> : EnumerableResponseBase<T>
{
    public PaginationData? Pagination { get; set; }
}

public class PaginationData
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
}
