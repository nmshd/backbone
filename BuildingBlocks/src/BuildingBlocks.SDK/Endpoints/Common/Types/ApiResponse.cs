using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class ApiResponse<TResult> : IResponse
{
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => (int)Status is >= 200 and < 300;

    [MemberNotNullWhen(false, nameof(Result))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => !IsSuccess;

    [MemberNotNullWhen(true, nameof(Pagination))]
    public bool IsPaginated => Pagination != null;

    public TResult? Result { get; set; }
    public ApiError? Error { get; set; }
    public PaginationData? Pagination { get; set; }
    public HttpStatusCode Status { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}
