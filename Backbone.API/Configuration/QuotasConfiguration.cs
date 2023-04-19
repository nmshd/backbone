using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Quotas.Application;

namespace Backbone.API.Configuration;

public class QuotasConfiguration
{
    public ApplicationOptions Application { get; set; } = new();

    [Required]
    public QuotasInfrastructure Infrastructure { get; set; } = new();
}

public class QuotasInfrastructure
{
    [Required]
    public SqlDatabaseConfig SqlDatabase { get; set; } = new();
}

public class SqlDatabaseConfig
{
    [Required]
    [MinLength(1)]
    [RegularExpression("SqlServer|Postgres")]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; set; } = string.Empty;
}