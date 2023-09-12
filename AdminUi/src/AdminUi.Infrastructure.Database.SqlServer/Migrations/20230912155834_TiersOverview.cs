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
                CREATE OR ALTER VIEW TierOverviews AS
                    SELECT 
                        TIERS.Id,
                        TIERS.Name,
                        IDENTITIES.NumberOfIdentities
                    FROM Devices.Tiers TIERS
                    LEFT JOIN (
                        SELECT
        		            TierId,
        		            COUNT(*) NumberOfIdentities
                        FROM Devices.Identities
                        GROUP BY TierId
                    ) as IDENTITIES ON IDENTITIES.TierId = TIERS.Id
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW TierOverviews """);
        }
    }
}
