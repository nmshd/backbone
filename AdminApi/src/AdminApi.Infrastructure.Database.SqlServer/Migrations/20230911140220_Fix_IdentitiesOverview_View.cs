using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations;

/// <inheritdoc />
public partial class Fix_IdentitiesOverview_View : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                CREATE OR ALTER VIEW IdentityOverviews AS
                    SELECT 
                        IDENTITIES.Address,
        	            IDENTITIES.CreatedAt,
        	            USERS.LastLoginAt,
        	            IDENTITIES.ClientId AS CreatedWithClient,
        	            DATAWALLETS.Version AS DatawalletVersion,
        	            IDENTITIES.IdentityVersion,
        	            TIERS.Id AS TierId,
        	            TIERS.Name AS TierName,
        	            DEVICES.NumberOfDevices
                    FROM Devices.Identities IDENTITIES
                    LEFT JOIN (
        	            SELECT 
        		            IdentityAddress,
        		            COUNT(*) NumberOfDevices
        	            FROM DEVICES.Devices
        	            GROUP BY IdentityAddress
                    ) as DEVICES ON DEVICES.IdentityAddress = IDENTITIES.Address
                    LEFT JOIN (
        	            SELECT 
        		            USERS.DeviceId,
        		            DEVICES.IdentityAddress,
        		            MAX(USERS.LastLoginAt) AS LastLoginAt
        	            FROM Devices.AspNetUsers USERS
        	            INNER JOIN Devices.Devices DEVICES ON DEVICES.Id = USERS.DeviceId
        	            GROUP BY USERS.DeviceId, DEVICES.IdentityAddress
                    ) AS USERS ON USERS.IdentityAddress = IDENTITIES.Address
                    LEFT JOIN Synchronization.Datawallets DATAWALLETS ON DATAWALLETS.Owner = IDENTITIES.Address
                    LEFT JOIN Devices.Tiers TIERS ON TIERS.Id = IDENTITIES.TierId
        """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE OR ALTER VIEW IdentityOverviews AS
                SELECT IDENTITIES.Address,
                IDENTITIES.CreatedAt,
                USERS.LastLoginAt,
                IDENTITIES.ClientId AS CreatedWithClient,
                DATAWALLETS.Version AS DatawalletVersion,
                IDENTITIES.IdentityVersion,
                TIERS.Id AS TierId,
                TIERS.Name AS TierName,
                COUNT(IDENTITIES.Address) OVER (PARTITION BY DEVICES.IdentityAddress) AS NumberOfDevices
                FROM Devices.Identities IDENTITIES
                LEFT JOIN Devices.Devices DEVICES ON DEVICES.IdentityAddress = IDENTITIES.Address
                LEFT JOIN (
                    SELECT 
                        DeviceId,
                        MAX(LastLoginAt) AS LastLoginAt
                        FROM Devices.AspNetUsers
                        GROUP BY DeviceId
                ) AS USERS ON USERS.DeviceId = DEVICES.Id
                LEFT JOIN Synchronization.Datawallets DATAWALLETS ON DATAWALLETS.Owner = IDENTITIES.Address
                LEFT JOIN Devices.Tiers TIERS ON TIERS.Id = IDENTITIES.TierId
        """);
    }
}
