using System.Net;

namespace Backbone.AdminUi.Tests.Integration.Models;

public class ODataResponse<T>
{
    public ODataResponseContent<T> Content { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}
