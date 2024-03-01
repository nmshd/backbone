using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ClientsOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW "ClientOverviews" AS
                    SELECT
                        CLIENTS."ClientId" AS "ClientId",
                        CLIENTS."DisplayName" AS "DisplayName",
                        CLIENTS."DefaultTier" AS "DefaultTier",
                        CLIENTS."CreatedAt" AS "CreatedAt",
                        (
        		            SELECT COUNT("ClientId") 
        		            FROM "Devices"."Identities"
        		            WHERE "ClientId" = CLIENTS."ClientId"
        	            ) AS "NumberOfIdentities"
                    FROM "Devices"."OpenIddictApplications" CLIENTS
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW "ClientOverviews" """);
        }
    }
}
