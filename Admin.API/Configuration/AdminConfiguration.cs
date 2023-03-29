using System.ComponentModel.DataAnnotations;

namespace Admin.API.Configuration;

public class AdminConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    public SwaggerUiConfiguration SwaggerUi { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        [Required]
        [MinLength(1)]
        public string JwtSigningCertificateSource { get; set; } = "";

        public string JwtSigningCertificate { get; set; } = "";

        [Required]
        [Range(60, 3600)]
        public int JwtLifetimeInSeconds { get; set; }
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = "";
        public string ExposedHeaders { get; set; } = "";
    }

    public class SwaggerUiConfiguration
    {
        [Required]
        public string TokenUrl { get; set; } = "";
    }

    public class ModulesConfiguration
    {

        [Required]
        public DevicesConfiguration Devices { get; set; } = new();

    }
}