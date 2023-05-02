using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;

namespace Backbone.API.Configuration;

public class BackboneConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    public SwaggerUiConfiguration SwaggerUi { get; set; } = new();

    [Required]
    public BackboneInfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
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

    public class BackboneInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
    }

    public class ModulesConfiguration
    {
        [Required]
        public ChallengesConfiguration Challenges { get; set; } = new();

        [Required]
        public DevicesConfiguration Devices { get; set; } = new();

        [Required]
        public QuotasConfiguration Quotas { get; set; } = new();

        [Required]
        public FilesConfiguration Files { get; set; } = new();

        [Required]
        public MessagesConfiguration Messages { get; set; } = new();

        [Required]
        public RelationshipsConfiguration Relationships { get; set; } = new();

        [Required]
        public SynchronizationConfiguration Synchronization { get; set; } = new();

        [Required]
        public TokensConfiguration Tokens { get; set; } = new();
    }
}