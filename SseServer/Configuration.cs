using System.ComponentModel.DataAnnotations;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string JwtSigningCertificate { get; set; } = "";

        [Required]
        [Range(60, 3600)]
        public int JwtLifetimeInSeconds { get; set; }
    }

    public class ModulesConfiguration
    {
        [Required]
        public Modules.Devices.ConsumerApi.Configuration Devices { get; set; }
    }
}
