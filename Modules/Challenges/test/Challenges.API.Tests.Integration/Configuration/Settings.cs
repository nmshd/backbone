using System.ComponentModel.DataAnnotations;

namespace Challenges.API.Tests.Integration.Configuration;
public static class Settings
{
    public class HttpConfiguration
    {
        [Required]
        public string BaseUrl { get; set; } = "";
        public ClientCredentialsConfiguration ClientCredentials { get; set; } = new();
    }

    public class ClientCredentialsConfiguration
    {
        [Required]
        public string ClientId { get; set; } = "";
        [Required]
        public string ClientSecret { get; set; } = "";
    }
}
