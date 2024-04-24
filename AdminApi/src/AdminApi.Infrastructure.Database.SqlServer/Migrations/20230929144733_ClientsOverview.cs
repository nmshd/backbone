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
                CREATE VIEW AdminUi.ClientOverviews AS
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW AdminUi.ClientOverviews """);
        }
    }
}
