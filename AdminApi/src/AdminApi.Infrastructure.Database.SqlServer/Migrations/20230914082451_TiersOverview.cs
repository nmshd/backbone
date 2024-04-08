using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class TiersOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR ALTER VIEW AdminUi.TierOverviews AS
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW TierOverviews """);
        }
    }
}
