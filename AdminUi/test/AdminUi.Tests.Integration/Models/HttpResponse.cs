using System.Net;

namespace AdminUi.Tests.Integration.Models;

public class HttpResponse<T>
{
    public ResponseContent<T> Content { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}

public class HttpResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public ErrorResponseContent? Content { get; set; }
}
