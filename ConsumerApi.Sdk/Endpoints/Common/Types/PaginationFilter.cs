using System.Collections.Specialized;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

public class PaginationFilter
{
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }

    public NameValueCollection ToQueryParameters() => new()
    {
        {"PageNumber", PageNumber.ToString()},
        {"PageSize", PageSize.ToString()}
    };
}
