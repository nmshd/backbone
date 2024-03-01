using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminUi.Infrastructure.Database.Postgres.Migrations;

/// <inheritdoc />
public partial class IdentitiesOverview : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE OR REPLACE VIEW "IdentityOverviews" AS
                SELECT IDENTITIES."Address",
                IDENTITIES."CreatedAt",
                USERS."LastLoginAt",
                IDENTITIES."ClientId" AS "CreatedWithClient",
                DATAWALLETS."Version" AS "DatawalletVersion",
                IDENTITIES."IdentityVersion",
                TIERS."Id" AS "TierId",
                TIERS."Name" AS "TierName",
                COUNT(IDENTITIES."Address") OVER (PARTITION BY DEVICES."IdentityAddress") AS "NumberOfDevices"
                FROM "Devices"."Identities" IDENTITIES
                LEFT JOIN "Devices"."Devices" DEVICES ON DEVICES."IdentityAddress" = IDENTITIES."Address"
                LEFT JOIN (
                    SELECT 
                        "DeviceId",
                        MAX("LastLoginAt") AS "LastLoginAt"
                    FROM "Devices"."AspNetUsers"
                    GROUP BY "DeviceId"
                ) AS USERS ON USERS."DeviceId" = DEVICES."Id"
                LEFT JOIN "Synchronization"."Datawallets" DATAWALLETS ON DATAWALLETS."Owner" = IDENTITIES."Address"
                LEFT JOIN "Devices"."Tiers" TIERS ON TIERS."Id" = IDENTITIES."TierId"
        """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(""" DROP VIEW "IdentityOverviews" """);
    }
}
