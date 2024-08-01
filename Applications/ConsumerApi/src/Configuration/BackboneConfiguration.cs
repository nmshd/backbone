using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;
using ChallengesConfiguration = Backbone.Modules.Challenges.ConsumerApi.Configuration;
using DevicesConfiguration = Backbone.Modules.Devices.ConsumerApi.Configuration;
using FilesConfiguration = Backbone.Modules.Files.ConsumerApi.Configuration;
using MessagesConfiguration = Backbone.Modules.Messages.ConsumerApi.Configuration;
using QuotasConfiguration = Backbone.Modules.Quotas.ConsumerApi.Configuration;
using RelationshipsConfiguration = Backbone.Modules.Relationships.ConsumerApi.Configuration;
using SynchronizationConfiguration = Backbone.Modules.Synchronization.ConsumerApi.Configuration;
using TokensConfiguration = Backbone.Modules.Tokens.ConsumerApi.Configuration;

namespace Backbone.ConsumerApi.Configuration;

public class BackboneConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    public SwaggerUiConfiguration SwaggerUi { get; set; } = new();

    [Required]
    public BackboneInfrastructureConfiguration Infrastructure { get; set; } = new();

    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        [Required]
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
        public bool Enabled { get; set; } = false;

        public string TokenUrl { get; set; } = "";
    }

    public class BackboneInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
    }
}

public class ModulesConfiguration
{
    public ChallengesConfiguration Challenges { get; set; } = new();
    public DevicesConfiguration Devices { get; set; } = new();
    public FilesConfiguration Files { get; set; } = new();
    public MessagesConfiguration Messages { get; set; } = new();
    public QuotasConfiguration Quotas { get; set; } = new();
    public RelationshipsConfiguration Relationships { get; set; } = new();
    public SynchronizationConfiguration Synchronization { get; set; } = new();
    public TokensConfiguration Tokens { get; set; } = new();
}
