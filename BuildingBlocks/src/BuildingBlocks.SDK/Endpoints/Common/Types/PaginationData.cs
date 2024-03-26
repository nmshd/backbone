namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class PaginationData
{
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public required int TotalPages { get; set; }
    public required int TotalRecords { get; set; }
}
