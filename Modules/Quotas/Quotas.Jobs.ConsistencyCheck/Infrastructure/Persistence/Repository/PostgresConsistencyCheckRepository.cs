using Dapper;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Repository;
internal class PostgresConsistencyCheckRepository : IConsistencyCheckRepository
{
    private readonly DapperRepositoryOptions _options;

    public PostgresConsistencyCheckRepository(IOptions<DapperRepositoryOptions> options)
    {
        _options = options.Value;
    }

    public Task<IEnumerable<string>> GetIdentitiesMissingFromDevices(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<string>> GetIdentitiesMissingFromQuotas(CancellationToken cancellationToken)
    {
        const string query = "";
        await using var connection = new SqlConnection(_options.ConnectionString);
        return await connection.QueryAsync<string>(new CommandDefinition(query, cancellationToken: cancellationToken));
    }

    public Task<IEnumerable<string>> GetTiersMissingFromDevices(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetTiersMissingFromQuotas(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
