using Challenges.Application;

namespace Backbone.API.Configuration;

public class ChallengesConfiguration
{
    public ApplicationOptions Application { get; set; } = new();
    public ChallengesInfrastructure Infrastructure { get; set; } = new();
}

public class ChallengesInfrastructure
{
    public SqlDatabase SqlDatabase { get; set; } = new();
}

public class SqlDatabase
{
    public string ConnectionString { get; set; } = "";
}