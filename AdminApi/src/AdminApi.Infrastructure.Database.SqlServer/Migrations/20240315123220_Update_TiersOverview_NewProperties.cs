using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Update_TiersOverview_NewProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR ALTER VIEW TierOverviews AS
                    SELECT
                        TIERS.Id,
                        TIERS.Name,
                        COUNT (IDENTITIES.TierId) AS NumberOfIdentities,
                        TIERS.CanBeUsedAsDefaultForClient,
                        TIERS.CanBeManuallyAssigned
                    FROM Devices.Tiers TIERS
                    LEFT JOIN Devices.Identities IDENTITIES
                    ON IDENTITIES.TierId = TIERS.Id
                    GROUP BY TIERS.Id, TIERS.Name, TIERS.CanBeUsedAsDefaultForClient, TIERS.CanBeManuallyAssigned
             """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR ALTER VIEW TierOverviews AS
                    SELECT
                       TIERS.Id,
                       TIERS.Name,
                       COUNT (IDENTITIES.TierId) AS NumberOfIdentities
                   FROM Devices.Tiers TIERS
                   LEFT JOIN Devices.Identities IDENTITIES
                   ON IDENTITIES.TierId = TIERS.Id
                   GROUP BY TIERS.Id, TIERS.Name
             """);
        }
    }
}
