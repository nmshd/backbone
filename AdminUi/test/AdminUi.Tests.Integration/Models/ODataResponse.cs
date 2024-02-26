using System.Net;

namespace Backbone.AdminUi.Tests.Integration.Models;

public class ODataResponse<T>
{
    public required ODataResponseContent<T> Content { get; set; }
    public required HttpStatusCode StatusCode { get; set; }
    public required bool IsSuccessStatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? RawContent { get; set; }
}
