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

    public TResult? Result { get; set; }
    public ConsumerApiError? Error { get; set; }
    public HttpStatusCode Status { get; set; }
}
