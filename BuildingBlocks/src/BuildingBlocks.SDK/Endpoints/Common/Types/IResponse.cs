using System.Net;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public interface IResponse
{
    bool IsSuccess { get; }
    bool IsError { get; }
    ApiError? Error { get; set; }
    HttpStatusCode Status { get; set; }
}
