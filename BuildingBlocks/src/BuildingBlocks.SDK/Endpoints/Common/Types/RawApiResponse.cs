using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class RawApiResponse
{
    [MemberNotNullWhen(true, nameof(Content))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error == null;

    [MemberNotNullWhen(false, nameof(Content))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => !IsSuccess;

    public byte[]? Content { get; set; }
    public ApiError? Error { get; set; }
    public required HttpStatusCode Status { get; set; }
}
