using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

public class RawConsumerApiResponse
{
    [MemberNotNullWhen(true, nameof(Content))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error == null;

    [MemberNotNullWhen(false, nameof(Content))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => !IsSuccess;

    public byte[]? Content { get; set; }
    public ConsumerApiError? Error { get; set; }
    public required HttpStatusCode Status { get; set; }
}
