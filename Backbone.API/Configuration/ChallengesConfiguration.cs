namespace Backbone.API.Configuration;

public class ChallengesConfiguration
{
    public Application Application { get; set; } = new();
    public ChallengesInfrastructure Infrastructure { get; set; } = new();
}

public class Application
{
}

public class ChallengesInfrastructure
{
    public SqlDatabase SqlDatabase { get; set; } = new();
}

public class SqlDatabase
{
    public string ConnectionString { get; set; } = "";
}