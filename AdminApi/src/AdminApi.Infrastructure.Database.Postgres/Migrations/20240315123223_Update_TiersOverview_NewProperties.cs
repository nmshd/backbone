using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Update_TiersOverview_NewProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE VIEW "TierOverviews" AS
                    SELECT
                        TIERS."Id" AS "Id",
                        TIERS."Name" AS "Name",
                        COUNT (IDENTITIES."TierId") AS "NumberOfIdentities",
                        TIERS."CanBeUsedAsDefaultForUser" AS "CanBeUsedAsDefaultForUser",
                        TIERS."CanBeManuallyAssigned" AS "CanBeManuallyAssigned"
                    FROM "Devices"."Tiers" TIERS
                    LEFT JOIN "Devices"."Identities" IDENTITIES
                    ON IDENTITIES."TierId" = TIERS."Id"
                    GROUP BY TIERS."Id", TIERS."Name", TIERS."CanBeUsedAsDefaultForUser", TIERS."CanBeManuallyAssigned"
             """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW "TierOverviews" """);
        }
    }
}
