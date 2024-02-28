using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

public class ConsumerApiResponse<TResult>
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
    public ConsumerApiError? Error { get; set; }
    public PaginationData? Pagination { get; set; }
    public HttpStatusCode Status { get; set; }
}

public class EmptyResponse { }

public class EmptyConsumerApiResponse : ConsumerApiResponse<EmptyResponse> { }

public class RawConsumerApiResponse
{
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error == null;

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => !IsSuccess;

    public required byte[] Content { get; set; }
    public ConsumerApiError? Error { get; set; }
    public required HttpStatusCode Status { get; set; }
}
