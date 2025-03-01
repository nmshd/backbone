namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class CachedApiResponse<TResult> : ApiResponse<TResult>
{
    public bool NotModified => Result == null;

    public string ETag { get; set; } = string.Empty;
}
