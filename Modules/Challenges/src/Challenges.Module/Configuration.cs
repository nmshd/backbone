using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Challenges.Application;

namespace Backbone.Modules.Challenges.Module;

public class Configuration
{
    public ApplicationOptions Application { get; set; } = new();

    [Required]
    public ChallengesInfrastructure Infrastructure { get; set; } = new();
}

public class ChallengesInfrastructure
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
