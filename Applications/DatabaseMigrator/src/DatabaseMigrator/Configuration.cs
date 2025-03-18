using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.DatabaseMigrator;

public class Configuration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = null!;
}

public class InfrastructureConfiguration
{
    [Required]
    public DatabaseConfiguration SqlDatabase { get; set; } = null!;
}
