using Backbone.Common.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Repository;
internal class SqlServerConsistencyCheckRepository : IConsistencyCheckRepository
{
    private readonly DapperRepositoryOptions _options;

    public SqlServerConsistencyCheckRepository(IOptions<DapperRepositoryOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IEnumerable<string>> GetIdentitiesMissingFromDevices(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT Address FROM Quotas.Identities
        EXCEPT
        SELECT d.Address FROM Devices.Identities d JOIN Quotas.Identities q ON d.Address = q.Address
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetIdentitiesMissingFromQuotas(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT Address FROM Devices.Identities
        EXCEPT
        SELECT d.Address FROM Devices.Identities d JOIN Quotas.Identities q ON d.Address = q.Address
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IdentityAddressTierQuotaDefinitionIdPair>> GetTierQuotasMissingFromIdentities(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT i.Address AS IdentityAddress, tqd.id AS TierQuotaDefinitionId
        FROM quotas.TierQuotaDefinitions tqd
        JOIN quotas.Tiers t ON t.Id = tqd.TierId
        JOIN quotas.Identities i ON i.TierId = t.id
        EXCEPT
        SELECT i.Address AS IdentityAddress, tq.DefinitionId AS TierQuotaDefinitionId
        FROM quotas.TierQuotas tq
        JOIN quotas.Identities i ON i.Address = tq.ApplyTo
        """;

        return await RunQueryAsync<IdentityAddressTierQuotaDefinitionIdPair>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetTiersMissingFromDevices(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT Id From Quotas.Tiers
        EXCEPT
        SELECT d.Id FROM Devices.Tiers d JOIN Quotas.Tiers q ON d.Id = q.Id
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetTiersMissingFromQuotas(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT Id From Devices.Tiers
        EXCEPT
        SELECT d.Id FROM Devices.Tiers d JOIN Quotas.Tiers q ON d.Id = q.Id
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IEnumerable<T>> RunQueryAsync<T>(string query, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_options.ConnectionString);
        return await connection.QueryAsync<T>(new CommandDefinition(query, cancellationToken: cancellationToken));
    }
}
