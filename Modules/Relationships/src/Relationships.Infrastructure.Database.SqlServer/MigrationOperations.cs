using System.Text;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer;

public static class MigrationOperations
{
    private const string FUNCTION_NAME_GET_NUMBER_OF_ACTIVE_RELATIONSHIPS_BETWEEN = "getNumberOfActiveRelationshipsBetween";

    public static void AddCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities(
        this MigrationBuilder migrationBuilder)
    {
        AddFunction(migrationBuilder);
        AddConstraint(migrationBuilder);
    }

    private static void AddFunction(MigrationBuilder migrationBuilder)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder
            .AppendLine($"CREATE FUNCTION [Relationships].[{FUNCTION_NAME_GET_NUMBER_OF_ACTIVE_RELATIONSHIPS_BETWEEN}] (@identityA varchar(36), @identityB varchar(36))")
            .AppendLine("RETURNS int")
            .AppendLine("AS BEGIN")
            .AppendLine(
                "return (SELECT COUNT(id) FROM [Relationships].[Relationships] r WITH(UPDLOCK, HOLDLOCK) WHERE ((r.[From] = @identityA AND r.[To] = @identityB) OR (r.[FROM] = @identityB AND r.[To] = @identityA)) AND r.Status IN (10, 20, 50))")
            .AppendLine("END");

        migrationBuilder.Sql(sqlBuilder.ToString());
    }

    private static void AddConstraint(MigrationBuilder migrationBuilder)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder
            .AppendLine("ALTER TABLE [Relationships].[Relationships]")
            .AppendLine($"WITH CHECK ADD CONSTRAINT {ConstraintNames.ONLY_ONE_ACTIVE_RELATIONSHIP_BETWEEN_TWO_IDENTITIES}")
            .AppendLine($"CHECK([Relationships].[{FUNCTION_NAME_GET_NUMBER_OF_ACTIVE_RELATIONSHIPS_BETWEEN}]([From], [To]) <= 1)");

        migrationBuilder.Sql(sqlBuilder.ToString());
    }

    public static void DeleteCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities(
        this MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($"ALTER TABLE [Relationships].[Relationships] DROP CONSTRAINT {ConstraintNames.ONLY_ONE_ACTIVE_RELATIONSHIP_BETWEEN_TWO_IDENTITIES}");
        migrationBuilder.Sql($"DROP FUNCTION [Relationships].[{FUNCTION_NAME_GET_NUMBER_OF_ACTIVE_RELATIONSHIPS_BETWEEN}]");
    }
}
