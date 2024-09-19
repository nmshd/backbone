using System.Collections.Specialized;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class PaginationFilter : IQueryParameterStorage
{
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }

    public NameValueCollection ToQueryParameters() => new()
    {
        {"PageNumber", PageNumber.ToString()},
        {"PageSize", PageSize.ToString()}
    };
}
