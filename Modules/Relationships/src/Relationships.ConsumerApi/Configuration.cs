using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Relationships.Application;

namespace Backbone.Modules.Relationships.ConsumerApi;

public class Configuration
{
    [Required]
    public ApplicationOptions Application { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public Dictionary<string, IEnumerable<PublicRelationshipTemplateReferenceDefinition>> PublicRelationshipTemplateReferences { get; set; } = [];

    public class InfrastructureConfiguration
    {
        [Required]
        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();

        public class SqlDatabaseConfiguration
        {
            [Required]
            [MinLength(1)]
            [RegularExpression("SqlServer|Postgres")]
            public string Provider { get; set; } = string.Empty;

            [Required]
            [MinLength(1)]
            public string ConnectionString { get; set; } = string.Empty;

            [Required]
            public bool EnableHealthCheck { get; set; } = true;
        }
    }

    public class PublicRelationshipTemplateReferenceDefinition
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string TruncatedReference { get; set; } = string.Empty;
    }
}
