using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Backbone.AdminApi.Sdk.Endpoints.Common.Types;

public class AdminApiResponse<TResult>
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
    public AdminApiError? Error { get; set; }
    public PaginationData? Pagination { get; set; }
    public HttpStatusCode Status { get; set; }
}

public class EmptyResponse;
