using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Backbone.Common.Infrastructure.Persistence.Repository;

public class PostgresMetricStatusesRepository : IMetricStatusesRepository
{
    private const string QUERY = """SELECT * FROM "MetricStatuses" WHERE "Owner" = @identityAddress AND "MetricKey" = ANY(@keys)""";

    private readonly DapperRepositoryOptions _options;

    public PostgresMetricStatusesRepository(IOptions<DapperRepositoryOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identityAddress, IEnumerable<MetricKey> keys)
    {
        await using var connection = new NpgsqlConnection(_options.ConnectionString);

        var metricStatuses = await connection.QueryAsync<MetricStatus>(QUERY, new
        {
            identityAddress = identityAddress.ToString(),
            keys = keys.Select(x => x.Value).ToArray()
        });

        return metricStatuses;
    }
}
