using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public sealed record OutboxDatabase(Type ContextType, string Module);

// Every operation uses its own short-lived scope/connection. No transaction crosses a broker call.
public sealed class OutboxStore(IServiceScopeFactory scopes)
{
    public async Task<List<OutboxMessage>> Claim(OutboxDatabase database, int batchSize, Guid lease, int leaseSeconds, CancellationToken cancellationToken)
    {
        await using var scope = scopes.CreateAsyncScope();
        var context = (DbContext)scope.ServiceProvider.GetRequiredService(database.ContextType);
        var table = Table(context);
        var postgres = context.Database.IsNpgsql();
        var sql = postgres
            ? $"""
               WITH candidates AS (
                   SELECT m."Id" FROM {table} m
                   WHERE m."PublishedAt" IS NULL AND m."FailedAt" IS NULL
                     AND m."NextAttemptAt" <= CURRENT_TIMESTAMP
                     AND (m."LeaseExpiresAt" IS NULL OR m."LeaseExpiresAt" <= CURRENT_TIMESTAMP)
                     AND NOT EXISTS (SELECT 1 FROM {table} p WHERE p."Stream" = m."Stream" AND ((p."SaveId" <> m."SaveId" AND (SELECT MIN(a."Sequence") FROM {table} a WHERE a."SaveId" = p."SaveId" AND a."Stream" = m."Stream") < (SELECT MIN(b."Sequence") FROM {table} b WHERE b."SaveId" = m."SaveId" AND b."Stream" = m."Stream")) OR (p."SaveId" = m."SaveId" AND p."Position" < m."Position")) AND p."PublishedAt" IS NULL)
                   ORDER BY m."NextAttemptAt", m."Sequence" LIMIT @batch FOR UPDATE OF m SKIP LOCKED
               )
               UPDATE {table} m SET "LeaseId" = @lease, "LeaseExpiresAt" = CURRENT_TIMESTAMP + @seconds * INTERVAL '1 second', "Attempts" = m."Attempts" + 1
               FROM candidates c WHERE m."Id" = c."Id" RETURNING m.*
               """
            : $"""
               ;WITH candidates AS (
                   SELECT TOP (@batch) m.* FROM {table} m WITH (UPDLOCK, READPAST, READCOMMITTEDLOCK)
                   WHERE m."PublishedAt" IS NULL AND m."FailedAt" IS NULL
                     AND m."NextAttemptAt" <= SYSUTCDATETIME()
                     AND (m."LeaseExpiresAt" IS NULL OR m."LeaseExpiresAt" <= SYSUTCDATETIME())
                     AND NOT EXISTS (SELECT 1 FROM {table} p WHERE p."Stream" = m."Stream" AND ((p."SaveId" <> m."SaveId" AND (SELECT MIN(a."Sequence") FROM {table} a WHERE a."SaveId" = p."SaveId" AND a."Stream" = m."Stream") < (SELECT MIN(b."Sequence") FROM {table} b WHERE b."SaveId" = m."SaveId" AND b."Stream" = m."Stream")) OR (p."SaveId" = m."SaveId" AND p."Position" < m."Position")) AND p."PublishedAt" IS NULL)
                   ORDER BY m."NextAttemptAt", m."Sequence"
               )
               UPDATE candidates SET "LeaseId" = @lease, "LeaseExpiresAt" = DATEADD(second, @seconds, SYSUTCDATETIME()), "Attempts" = "Attempts" + 1 OUTPUT INSERTED.*;
               """;
        await context.Database.OpenConnectionAsync(cancellationToken);
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = context.Database.GetCommandTimeout() ?? 30;
        Add(command, "batch", batchSize);
        Add(command, "lease", lease);
        Add(command, "seconds", leaseSeconds);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        List<OutboxMessage> result = [];
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new OutboxMessage
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                EventId = reader.GetString(reader.GetOrdinal("EventId")),
                EventName = reader.GetString(reader.GetOrdinal("EventName")),
                EventType = reader.GetString(reader.GetOrdinal("EventType")),
                Payload = reader.GetString(reader.GetOrdinal("Payload")),
                CreatedAt = DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("CreatedAt")), DateTimeKind.Utc),
                Attempts = reader.GetInt32(reader.GetOrdinal("Attempts")),
                CorrelationId = reader.GetString(reader.GetOrdinal("CorrelationId")),
                TraceParent = Text(reader, "TraceParent"),
                TraceState = Text(reader, "TraceState")
            });
        }
        return result;
    }

    public Task<int> Renew(OutboxDatabase database, Guid lease, int seconds, CancellationToken token) => Execute(database,
        (table, postgres) => $"UPDATE {table} SET \"LeaseExpiresAt\" = " + (postgres ? "CURRENT_TIMESTAMP + @seconds * INTERVAL '1 second'" : "DATEADD(second, @seconds, SYSUTCDATETIME())") +
                             " WHERE \"LeaseId\" = @lease AND \"PublishedAt\" IS NULL AND \"FailedAt\" IS NULL", token, ("lease", lease), ("seconds", seconds));

    public Task<int> Own(OutboxDatabase database, Guid id, Guid lease, int seconds, CancellationToken token) => Execute(database,
        (table, postgres) => $"UPDATE {table} SET \"LeaseExpiresAt\" = " + (postgres ? "CURRENT_TIMESTAMP + @seconds * INTERVAL '1 second'" : "DATEADD(second, @seconds, SYSUTCDATETIME())") +
                             $" WHERE \"Id\" = @id AND \"LeaseId\" = @lease AND \"LeaseExpiresAt\" > {(postgres ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")}",
        token, ("id", id), ("lease", lease), ("seconds", seconds));

    public Task<int> Complete(OutboxDatabase database, Guid id, Guid lease, CancellationToken token) => Execute(database,
        (table, postgres) => $"UPDATE {table} SET \"PublishedAt\" = {(postgres ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")}, \"LeaseId\" = NULL, \"LeaseExpiresAt\" = NULL, \"LastError\" = NULL WHERE \"Id\" = @id AND \"LeaseId\" = @lease",
        token, ("id", id), ("lease", lease));

    public Task<int> Fail(OutboxDatabase database, OutboxMessage message, Guid lease, OutboxOptions options, Exception error, CancellationToken token) => Execute(database,
        (table, postgres) => $"UPDATE {table} SET \"NextAttemptAt\" = " + (postgres ? "CURRENT_TIMESTAMP + @delay * INTERVAL '1 second'" : "DATEADD(second, @delay, SYSUTCDATETIME())") +
                             $", \"FailedAt\" = CASE WHEN @terminal = 1 THEN {(postgres ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")} ELSE NULL END, \"LastError\" = @error, \"LeaseId\" = NULL, \"LeaseExpiresAt\" = NULL WHERE \"Id\" = @id AND \"LeaseId\" = @lease",
        token, ("id", message.Id), ("lease", lease), ("delay", (int)Math.Ceiling(options.RetryDelay(message.Attempts).TotalSeconds)),
        ("terminal", message.Attempts >= options.MaxAttempts ? 1 : 0), ("error", error.GetType().Name));

    public async Task<(long Pending, double Age)> Backlog(OutboxDatabase database, CancellationToken token)
    {
        await using var scope = scopes.CreateAsyncScope();
        var context = (DbContext)scope.ServiceProvider.GetRequiredService(database.ContextType);
        var query = context.Set<OutboxMessage>().Where(x => x.PublishedAt == null);
        var statistics = await query.GroupBy(x => 1).Select(g => new { Count = g.LongCount(), Oldest = g.Min(x => x.CreatedAt) }).SingleOrDefaultAsync(token);
        return statistics == null ? (0, 0) : (statistics.Count, Math.Max(0, (DateTime.UtcNow - statistics.Oldest).TotalSeconds));
    }

    private async Task<int> Execute(OutboxDatabase database, Func<string, bool, string> sql, CancellationToken token, params (string Name, object Value)[] parameters)
    {
        await using var scope = scopes.CreateAsyncScope();
        var context = (DbContext)scope.ServiceProvider.GetRequiredService(database.ContextType);
        await context.Database.OpenConnectionAsync(token);
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql(Table(context), context.Database.IsNpgsql());
        command.CommandTimeout = context.Database.GetCommandTimeout() ?? 30;
        foreach (var (name, value) in parameters)
            Add(command, name, value);
        return await command.ExecuteNonQueryAsync(token);
    }

    private static string Table(DbContext context)
    {
        if (!context.Database.IsNpgsql() && !context.Database.IsSqlServer())
            throw new NotSupportedException("The outbox dispatcher requires PostgreSQL or SQL Server.");
        var entity = context.Model.FindEntityType(typeof(OutboxMessage))!;
        var helper = context.GetService<ISqlGenerationHelper>();
        return helper.DelimitIdentifier(entity.GetTableName()!, entity.GetSchema());
    }

    private static void Add(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static string? Text(DbDataReader reader, string column) => reader.IsDBNull(reader.GetOrdinal(column)) ? null : reader.GetString(reader.GetOrdinal(column));
}
