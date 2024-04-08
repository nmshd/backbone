using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Backbone.Common.Infrastructure.Persistence.Repository;

public class SqlServerMetricStatusesRepository : IMetricStatusesRepository
{
    private const string QUERY = "SELECT * FROM MetricStatuses WHERE Owner = @identityAddress AND MetricKey IN @keys";

    private readonly DapperRepositoryOptions _options;

    public SqlServerMetricStatusesRepository(IOptions<DapperRepositoryOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identityAddress, IEnumerable<MetricKey> keys)
    {
        await using var connection = new SqlConnection(_options.ConnectionString);

        var metricStatuses = await connection.QueryAsync<MetricStatus>(QUERY, new
        {
            identityAddress = identityAddress.ToString(),
            keys = keys.Select(x => x.Value).ToArray()
        });

        return metricStatuses;
    }
}
