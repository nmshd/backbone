using System.ComponentModel.DataAnnotations;

namespace AdminApi.Tests.Integration.Utils.Configuration;

public class HttpConfiguration
{
    [Required]
    public string BaseUrl { get; set; } = "";
}
