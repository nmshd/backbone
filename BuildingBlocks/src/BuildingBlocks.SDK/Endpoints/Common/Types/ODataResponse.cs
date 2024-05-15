using System.Net;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class ODataResponse<TResult>
{
    public TResult? Value { get; set; }
    public string? ContentType { get; set; }

    public ApiResponse<TResult> ToApiResponse(HttpStatusCode status, string? rawContent) => new()
    {
        Result = Value,
        ContentType = ContentType,
        Status = status,
        RawContent = rawContent
    };
}
