using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Challenges.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public SqlDatabase SqlDatabase { get; set; } = new();
}

public class SqlDatabase
{
    [Required]
    [MinLength(1)]
    [RegularExpression("SqlServer|Postgres")]
    public string Provider { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; set; } = null!;

    [Required]
    public bool EnableHealthCheck { get; set; } = true;
}
