using System.Net;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class ODataResponse<TResult>
{
    public TResult? Value { get; set; }

    public ApiResponse<TResult> ToApiResponse(HttpStatusCode status) => new() { Result = Value, Status = status };
}
