using System.ComponentModel.DataAnnotations;

namespace Backbone.DatabaseMigrator;

public class Configuration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = null!;
}

public class InfrastructureConfiguration
{
    [Required]
    public SqlDatabaseConfiguration SqlDatabase { get; set; } = null!;
}

public class SqlDatabaseConfiguration
{
    [Required]
    public string Provider { get; set; } = null!;

    [Required]
    public string ConnectionString { get; set; } = null!;

    [Range(1, Int32.MaxValue)]
    public int CommandTimeout { get; set; } = 300;
}
