using System.ComponentModel.DataAnnotations;

namespace Backbone.ConsumerApi.Tests.Integration.Configuration;

public class HttpConfiguration
{
    [Required]
    public required ClientCredentialsConfiguration ClientCredentials { get; set; }
}

public class ClientCredentialsConfiguration
{
    [Required]
    public required string ClientId { get; set; }

    [Required]
    public required string ClientSecret { get; set; }
}
