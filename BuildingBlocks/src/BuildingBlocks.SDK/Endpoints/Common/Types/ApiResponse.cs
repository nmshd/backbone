using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class ApiResponse<TResult>
{
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error == null;

    [MemberNotNullWhen(false, nameof(Result))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => !IsSuccess;

    [MemberNotNullWhen(true, nameof(Pagination))]
    public bool IsPaginated => Pagination != null;

    public TResult? Result { get; set; }
    public ApiError? Error { get; set; }
    public PaginationData? Pagination { get; set; }
    public HttpStatusCode Status { get; set; }
    public string? RawContent { get; set; }
}

public class ApiResponse
{
    public HttpStatusCode Status { get; set; }
    public ApiError? Content { get; set; }
}

public class ResponseContent<TResult>
{
    public TResult? Result { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public ApiError? Error { get; set; }
}
