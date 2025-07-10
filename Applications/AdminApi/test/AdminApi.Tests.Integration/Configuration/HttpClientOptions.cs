using System.ComponentModel.DataAnnotations;

namespace Backbone.AdminApi.Tests.Integration.Configuration;

public class HttpClientOptions
{
    [Required]
    public required string ApiKey { get; set; }
}
