using System.Data;
using Dapper;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Enmeshed.Common.Infrastructure.Persistence.Repository;

public class MetricStatusesRepository : IMetricStatusesRepository
{
    private const string SQLSERVER = "SqlServer";
    private const string POSTGRES = "Postgres";

    private readonly MetricStatusesRepositoryOptions _options;
    private readonly string _query;


    public MetricStatusesRepository(IOptions<MetricStatusesRepositoryOptions> options)
    {
        _options = options.Value;

        const string query = "SELECT * FROM MetricStatuses WHERE Owner = @identityAddress AND MetricKey IN @keys";
        const string queryPostgres = """SELECT * FROM "MetricStatuses" WHERE "Owner" = @identityAddress AND "MetricKey" = ANY(@keys)""";
        _query = _options.Provider == POSTGRES ? queryPostgres : query;
    }

    private IDbConnection CreateConnection()
    {
        return _options.Provider switch
        {
            SQLSERVER => new SqlConnection(_options.ConnectionString),
            POSTGRES => new NpgsqlConnection(_options.ConnectionString),
            _ => throw new NotSupportedException($"Unsupported database provider: '{_options.Provider}'")
        };
    }

    public async Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identityAddress, IEnumerable<MetricKey> keys)
    {
        var connection = CreateConnection();
        var metricStatuses = await connection.QueryAsync<MetricStatus>(_query, new
        {
            identityAddress = identityAddress.ToString(),
            keys = keys.Select(x => x.Value).ToArray()
        });

        connection.Dispose();

        return metricStatuses;
    }
}

public class MetricStatusesRepositoryOptions
{
    public string ConnectionString = string.Empty;
    public string Provider = string.Empty;
}
