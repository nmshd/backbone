using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres;

public static class MigrationOperations
{
    private const string INDEX_NAME_ONLY_ONE_ACTIVE_DELETION_PROCESS = "IX_only_one_active_deletion_process";

    public static void AddCheckConstraintForAtMostOneActiveDeletionProcessPerIdentity(this MigrationBuilder migrationBuilder)
    {
        AddIndex(migrationBuilder);
    }

    private static void AddIndex(MigrationBuilder migrationBuilder)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder
            .AppendLine($"CREATE UNIQUE INDEX {INDEX_NAME_ONLY_ONE_ACTIVE_DELETION_PROCESS}")
            .AppendLine(@"ON ""Devices"".""IdentityDeletionProcesses""(""IdentityAddress"")")
            .AppendLine(@"WHERE ""Status"" = 1");

        migrationBuilder.Sql(sqlBuilder.ToString());
    }

    public static void DeleteCheckConstraintForAtMostOneActiveDeletionProcessPerIdentity(this MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($"DROP Index \"Devices\".{INDEX_NAME_ONLY_ONE_ACTIVE_DELETION_PROCESS}");
    }
}
