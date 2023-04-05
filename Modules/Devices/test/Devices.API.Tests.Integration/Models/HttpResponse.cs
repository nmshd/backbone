using System.Net;

namespace Devices.API.Tests.Integration.Models;
public class HttpResponse<T>
{
    public T? Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? Content { get; set; }
}
