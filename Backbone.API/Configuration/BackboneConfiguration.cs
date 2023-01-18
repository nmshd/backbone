using Backbone.Infrastructure.EventBus;

namespace Backbone.API.Configuration;

public class BackboneConfiguration
{
    public AuthenticationConfiguration Authentication { get; set; } = new();
    public HttpConfiguration Http { get; set; } = new();
    public BackboneInfrastructureConfiguration Infrastructure { get; set; } = new();
    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string ValidIssuer { get; set; }
        public string JwtSigningCertificate { get; set; }
    }

    public class HttpConfiguration
    {
        public CorsConfiguration Cors { get; set; } = new();

        public class CorsConfiguration
        {
            public string AllowedOrigins { get; set; } = "";
            public string ExposedHeaders { get; set; } = "";
        }
    }

    public class BackboneInfrastructureConfiguration
    {
        public EventBusConfiguration EventBus { get; set; } = new();
    }

    public class ModulesConfiguration
    {
        public ChallengesConfiguration Challenges { get; set; } = new();
        public FilesConfiguration Files { get; set; } = new();
        public MessagesConfiguration Messages { get; set; } = new();
        public RelationshipsConfiguration Relationships { get; set; } = new();
        public SynchronizationConfiguration Synchronization { get; set; } = new();
        public TokensConfiguration Tokens { get; set; } = new();
    }
}