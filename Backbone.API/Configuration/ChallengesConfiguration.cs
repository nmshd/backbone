using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Challenges.Application;

namespace Backbone.API.Configuration;

public class ChallengesConfiguration
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
    public string Provider { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; set; } = string.Empty;
}