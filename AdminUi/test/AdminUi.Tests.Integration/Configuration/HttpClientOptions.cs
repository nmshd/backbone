using System.ComponentModel.DataAnnotations;

namespace AdminUi.Tests.Integration.Configuration;

public class HttpClientOptions
{
    [Required]
    public string BaseUrl { get; set; } = "";

    [Required]
    public string ApiKey { get; set; } = "";
}
