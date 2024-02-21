using System.Net;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class HttpResponse<T>
{
    public required ResponseContent<T> Content { get; set; }
    public string? HttpMethod { get; set; }
    public required HttpStatusCode StatusCode { get; set; }
    public required bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}

public class HttpResponse
{
    public required ErrorResponseContent? Content { get; set; }
    public string? HttpMethod { get; set; }
    public required HttpStatusCode StatusCode { get; set; }
    public required bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}
