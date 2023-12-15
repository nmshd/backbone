﻿using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Synchronization.Application;

namespace Backbone.Modules.Synchronization.ConsumerApi;

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

        public BlobStorageConfiguration? BlobStorage { get; set; }

        public class BlobStorageConfiguration
        {
            [Required]
            [MinLength(1)]
            [RegularExpression("Azure|GoogleCloud")]
            public string CloudProvider { get; set; } = string.Empty;

            public string ConnectionInfo { get; set; } = string.Empty;

            public string ContainerName { get; set; } = string.Empty;
        }

        public class SqlDatabaseConfiguration
        {
            [Required]
            [MinLength(1)]
            [RegularExpression("SqlServer|Postgres")]
            public string Provider { get; set; } = string.Empty;

            [Required]
            [MinLength(1)]
            public string ConnectionString { get; set; } = string.Empty;
        }
    }
}
