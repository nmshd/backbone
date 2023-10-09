using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ClientsOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW ClientOverviews AS
                    SELECT
                        CLIENTS.ClientId,
                        CLIENTS.DisplayName,
                        CLIENTS.DefaultTier,
                        CLIENTS.CreatedAt,
                        COUNT (IDENTITIES.ClientId) AS NumberOfIdentities
                    FROM Devices.OpenIddictApplications CLIENTS
                    LEFT JOIN Devices.Identities IDENTITIES
                    ON IDENTITIES.ClientId = CLIENTS.ClientId
                    GROUP BY CLIENTS.ClientId, CLIENTS.DisplayName, CLIENTS.DefaultTier, CLIENTS.CreatedAt
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW ClientOverviews """);
        }
    }
}
