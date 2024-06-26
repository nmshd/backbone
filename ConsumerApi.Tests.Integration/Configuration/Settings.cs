using System.ComponentModel.DataAnnotations;

namespace Backbone.ConsumerApi.Tests.Integration.Configuration;

public class HttpConfiguration
{
    [Required]
    public ClientCredentialsConfiguration ClientCredentials { get; set; } = new();
}

public class ClientCredentialsConfiguration
{
    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;
}
