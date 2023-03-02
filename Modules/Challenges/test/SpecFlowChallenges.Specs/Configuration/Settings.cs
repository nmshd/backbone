using System.ComponentModel.DataAnnotations;

namespace SpecFlowChallenges.Specs.Configuration;
internal class Settings
{
    public HttpConfiguration Request { get; set; } = new();

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
