using Backbone.Common.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Repository;
internal class PostgresConsistencyCheckRepository : IConsistencyCheckRepository
{
    private readonly DapperRepositoryOptions _options;

    public PostgresConsistencyCheckRepository(IOptions<DapperRepositoryOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IEnumerable<string>> GetIdentitiesMissingFromDevices(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT "Address" FROM "Quotas"."Identities"
        EXCEPT
        SELECT d."Address" FROM "Devices"."Identities" d JOIN "Quotas"."Identities" q ON d."Address" = q."Address"
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetIdentitiesMissingFromQuotas(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT "Address" FROM "Devices"."Identities"
        EXCEPT
        SELECT d."Address" FROM "Devices"."Identities" d JOIN "Quotas"."Identities" q ON d."Address" = q."Address"
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IdentityAddressTierQuotaDefinitionIdPair>> GetTierQuotasMissingFromIdentities(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT i."Address" AS "IdentityAddress", tqd."Id" AS "TierQuotaDefinitionId"
        FROM "Quotas"."TierQuotaDefinitions" tqd
        JOIN "Quotas"."Tiers" t ON t."Id" = tqd."TierId"
        JOIN "Quotas"."Identities" i ON i."TierId" = t."Id"
        EXCEPT
        SELECT i."Address" AS "IdentityAddress", tq."DefinitionId" AS "TierQuotaDefinitionId"
        FROM "Quotas"."TierQuotas" tq
        JOIN "Quotas"."Identities" i ON i."Address" = tq."ApplyTo"
        """;

        return await RunQueryAsync<IdentityAddressTierQuotaDefinitionIdPair>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetTiersMissingFromDevices(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT "Id" From "Quotas"."Tiers"
        EXCEPT
        SELECT d."Id" FROM "Devices"."Tiers" d JOIN "Quotas"."Tiers" q ON d."Id" = q."Id"
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetTiersMissingFromQuotas(CancellationToken cancellationToken)
    {
        const string query = """
        SELECT "Id" From "Devices"."Tiers"
        EXCEPT
        SELECT d."Id" FROM "Devices"."Tiers" d JOIN "Quotas"."Tiers" q ON d."Id" = q."Id"
        """;

        return await RunQueryAsync<string>(query, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IEnumerable<T>> RunQueryAsync<T>(string query, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_options.ConnectionString);
        return await connection.QueryAsync<T>(new CommandDefinition(query, cancellationToken: cancellationToken));
    }
}
