using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Files.Application;

namespace Backbone.Modules.Files.ConsumerApi;

public class Configuration
{
    [Required]
    public ApplicationOptions Application { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public class InfrastructureConfiguration
    {
        [Required]
        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();

        [Required]
        public BlobStorageOptions BlobStorage { get; set; } = new();

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
}
