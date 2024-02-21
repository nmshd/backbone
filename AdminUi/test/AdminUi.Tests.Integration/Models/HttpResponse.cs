using System.Net;

namespace Backbone.AdminUi.Tests.Integration.Models;

public class HttpResponse<T>
{
    public required ResponseContent<T> Content { get; set; }
    public required HttpStatusCode StatusCode { get; set; }
    public required bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}

public class HttpResponse
{
    public required HttpStatusCode StatusCode { get; set; }
    public required bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public required ErrorResponseContent? Content { get; set; }
}

public class Cookie
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}
