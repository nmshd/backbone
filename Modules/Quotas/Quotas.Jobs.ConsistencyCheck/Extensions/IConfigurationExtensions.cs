using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Extensions;

internal static class IEnumerableExtensions
{
    public static (IEnumerable<T> intersection, IEnumerable<T> aExceptIntersection, IEnumerable<T> bExceptIntersection) Distribute<T>(IEnumerable<T> a, IEnumerable<T> b) where T : class
    {
        var intersection = a.Intersect(b);
        var aExceptIntersection = a.Except(intersection);
        var bExceptIntersection = b.Except(intersection);
        return new(intersection, aExceptIntersection, bExceptIntersection);
    }
}

internal static class IConfigurationExtensions
{
    public static SqlDatabaseConfiguration GetSqlDatabaseConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("SqlDatabase").Get<SqlDatabaseConfiguration>() ?? new SqlDatabaseConfiguration();
    }
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
