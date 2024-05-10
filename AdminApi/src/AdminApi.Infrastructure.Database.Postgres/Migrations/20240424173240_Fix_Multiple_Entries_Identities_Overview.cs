using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Multiple_Entries_Identities_Overview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                     CREATE OR REPLACE VIEW "AdminUi"."IdentityOverviews" AS
                         SELECT
                             IDENTITIES."Address",
                            IDENTITIES."CreatedAt",
                            USERS."LastLoginAt",
                            IDENTITIES."ClientId" AS "CreatedWithClient",
                            DATAWALLETS."Version" AS "DatawalletVersion",
                            IDENTITIES."IdentityVersion",
                            TIERS."Id" AS "TierId",
                            TIERS."Name" AS "TierName",
                            DEVICES."NumberOfDevices"
                         FROM "Devices"."Identities" IDENTITIES
                         LEFT JOIN (
                            SELECT
                                "IdentityAddress",
                                COUNT(*) AS "NumberOfDevices"
                            FROM "Devices"."Devices"
                            GROUP BY "IdentityAddress"
                         ) as DEVICES ON DEVICES."IdentityAddress" = IDENTITIES."Address"
                         LEFT JOIN (
                            SELECT
                                DEVICES."IdentityAddress",
                                MAX(USERS."LastLoginAt") AS "LastLoginAt"
                            FROM "Devices"."AspNetUsers" USERS
                            INNER JOIN "Devices"."Devices" DEVICES ON DEVICES."Id" = USERS."DeviceId"
                            GROUP BY DEVICES."IdentityAddress"
                         ) AS USERS ON USERS."IdentityAddress" = IDENTITIES."Address"
                         LEFT JOIN "Synchronization"."Datawallets" DATAWALLETS ON DATAWALLETS."Owner" = IDENTITIES."Address"
                         LEFT JOIN "Devices"."Tiers" TIERS ON TIERS."Id" = IDENTITIES."TierId"
             """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                     CREATE OR REPLACE VIEW "AdminUi"."IdentityOverviews" AS
                         SELECT
                             IDENTITIES."Address",
                            IDENTITIES."CreatedAt",
                            USERS."LastLoginAt",
                            IDENTITIES."ClientId" AS "CreatedWithClient",
                            DATAWALLETS."Version" AS "DatawalletVersion",
                            IDENTITIES."IdentityVersion",
                            TIERS."Id" AS "TierId",
                            TIERS."Name" AS "TierName",
                            DEVICES."NumberOfDevices"
                         FROM "Devices"."Identities" IDENTITIES
                         LEFT JOIN (
                            SELECT
                                "IdentityAddress",
                                COUNT(*) AS "NumberOfDevices"
                            FROM "Devices"."Devices"
                            GROUP BY "IdentityAddress"
                         ) as DEVICES ON DEVICES."IdentityAddress" = IDENTITIES."Address"
                         LEFT JOIN (
                            SELECT
                                USERS."DeviceId",
                                DEVICES."IdentityAddress",
                                MAX(USERS."LastLoginAt") AS "LastLoginAt"
                            FROM "Devices"."AspNetUsers" USERS
                            INNER JOIN "Devices"."Devices" DEVICES ON DEVICES."Id" = USERS."DeviceId"
                            GROUP BY USERS."DeviceId", DEVICES."IdentityAddress"
                         ) AS USERS ON USERS."IdentityAddress" = IDENTITIES."Address"
                         LEFT JOIN "Synchronization"."Datawallets" DATAWALLETS ON DATAWALLETS."Owner" = IDENTITIES."Address"
                         LEFT JOIN "Devices"."Tiers" TIERS ON TIERS."Id" = IDENTITIES."TierId"
             """);
        }
    }
}
