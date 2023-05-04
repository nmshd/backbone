using System.Net;

namespace Tokens.API.Tests.Integration.Models;
public class HttpResponse<T>
{
    public ResponseContent<T> Content { get; set; }
    public string? HttpMethod { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}
