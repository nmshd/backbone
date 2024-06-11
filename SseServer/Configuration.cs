using System.ComponentModel.DataAnnotations;
using DevicesConfiguration = Backbone.Modules.Devices.ConsumerApi.Configuration;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string JwtSigningCertificate { get; set; } = "";
    }

    public class ModulesConfiguration
    {
        [Required]
        public DevicesConfiguration Devices { get; set; } = new();
    }
}
