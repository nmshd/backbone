using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations;

/// <inheritdoc />
public partial class IdentitiesOverview : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE OR ALTER VIEW AdminUi.IdentityOverviews\r\nAS\r\n\tSELECT IDENTITIES.Address,\r\n\tIDENTITIES.CreatedAt,\r\n\tUSERS.LastLoginAt,\r\n\tIDENTITIES.ClientId AS CreatedWithClient,\r\n\tDATAWALLETS.Version AS DatawalletVersion,\r\n\tIDENTITIES.IdentityVersion,\r\n\tTIERS.Id AS TierId,\r\n\tTIERS.Name AS TierName,\r\n\tCOUNT(IDENTITIES.Address) OVER (PARTITION BY DEVICES.IdentityAddress) AS NumberOfDevices\r\n\tFROM Devices.Identities IDENTITIES\r\n\tLEFT JOIN Devices.Devices DEVICES ON DEVICES.IdentityAddress = IDENTITIES.Address\r\n\tLEFT JOIN (\r\n\t\tSELECT \r\n\t\t\tDeviceId,\r\n\t\t\tMAX(LastLoginAt) AS LastLoginAt\r\n\t\tFROM Devices.AspNetUsers\r\n\t\tGROUP BY DeviceId\r\n\t) AS USERS ON USERS.DeviceId = DEVICES.Id\r\n\tLEFT JOIN Synchronization.Datawallets DATAWALLETS ON DATAWALLETS.Owner = IDENTITIES.Address\r\n\tLEFT JOIN Devices.Tiers TIERS ON TIERS.Id = IDENTITIES.TierId;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP VIEW AdminUi.IdentityOverviews;");
    }
}
