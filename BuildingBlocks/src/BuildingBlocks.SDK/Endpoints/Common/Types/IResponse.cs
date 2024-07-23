using System.Net;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public interface IResponse
{
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public ApiError? Error { get; set; }
    public HttpStatusCode Status { get; set; }
}
