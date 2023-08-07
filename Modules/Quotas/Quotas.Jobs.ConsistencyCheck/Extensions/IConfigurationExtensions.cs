using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Extensions;

internal static class IEnumerableExtensions
{
    /// <summary>
    /// Given two sets A & B, distributes them in three parts:
    /// </summary>
    /// <typeparam name="T">The type of the Enumerable to be distributed</typeparam>
    /// <param name="a">Set A</param>
    /// <param name="b">Set B</param>
    /// <returns>
    /// intersection - A ∩ B
    /// aExceptIntersection - A \ A ∩ B
    /// bExceptIntersection - B \ A ∩ B
    /// </returns>
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
