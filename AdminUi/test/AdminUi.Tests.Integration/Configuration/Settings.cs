using System.ComponentModel.DataAnnotations;

namespace AdminUi.Tests.Integration.Configuration;

public class HttpConfiguration
{
    [Required]
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
}
