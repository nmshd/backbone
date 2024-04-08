using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UseTierDTOInIdentitiesAndClientsOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR ALTER VIEW AdminUi.ClientOverviews AS
                    SELECT
                        CLIENTS.ClientId,
                        CLIENTS.DisplayName,
                        CLIENTS.DefaultTier AS DefaultTierId,
                        TIERS.Name AS DefaultTierName,
                        CLIENTS.CreatedAt,
                        (
        		            SELECT COUNT(ClientId) 
        		            FROM Devices.Identities
        		            WHERE ClientId = CLIENTS.ClientId
        	            ) AS NumberOfIdentities
                    FROM Devices.OpenIddictApplications CLIENTS
                    LEFT JOIN Devices.Tiers TIERS
                    ON TIERS.Id = CLIENTS.DefaultTier
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR ALTER VIEW ClientOverviews AS
                    SELECT
                        CLIENTS.ClientId,
                        CLIENTS.DisplayName,
                        CLIENTS.DefaultTier,
                        CLIENTS.CreatedAt,
                        (
        		            SELECT COUNT(ClientId) 
        		            FROM Devices.Identities
        		            WHERE ClientId = CLIENTS.ClientId
        	            ) AS NumberOfIdentities
                    FROM Devices.OpenIddictApplications CLIENTS
        """);
        }
    }
}
