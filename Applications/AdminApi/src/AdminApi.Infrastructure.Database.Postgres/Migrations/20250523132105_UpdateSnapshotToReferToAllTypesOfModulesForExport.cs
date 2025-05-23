using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSnapshotToReferToAllTypesOfModulesForExport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW \"AdminUi\".\"IdentityOverviews\"");
            migrationBuilder.Sql("DROP VIEW \"AdminUi\".\"RelationshipOverviews\"");
            migrationBuilder.Sql("DROP VIEW \"AdminUi\".\"TierOverviews\"");
            migrationBuilder.Sql("DROP VIEW \"AdminUi\".\"MessageOverviews\"");
            migrationBuilder.Sql("DROP VIEW \"AdminUi\".\"ClientOverviews\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                    CREATE VIEW "AdminUi"."IdentityOverviews" AS
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

            migrationBuilder.Sql(
                """
                    CREATE VIEW "AdminUi"."RelationshipOverviews" AS
                        SELECT
                            "Relationships"."From" AS "From",
                            "Relationships"."To" AS "To",
                            "Relationships"."RelationshipTemplateId" AS "RelationshipTemplateId",
                            "Relationships"."Status" AS "Status",
                            "AuditLog1"."CreatedAt" AS "CreatedAt",
                            "AuditLog1"."CreatedByDevice" AS "CreatedByDevice",
                            "AuditLog2"."CreatedAt" AS "AnsweredAt",
                            "AuditLog2"."CreatedByDevice" AS "AnsweredByDevice"
                        FROM "Relationships"."Relationships" AS "Relationships"
                        LEFT JOIN "Relationships"."RelationshipAuditLog" AS "AuditLog1"
                            ON "Relationships"."Id" = "AuditLog1"."RelationshipId" AND "AuditLog1"."Reason" = 0
                        LEFT JOIN "Relationships"."RelationshipAuditLog" AS "AuditLog2"
                            ON "Relationships"."Id" = "AuditLog2"."RelationshipId" AND "AuditLog2"."Reason" = 1
                """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW "AdminUi"."TierOverviews" AS
                        SELECT
                            TIERS."Id" AS "Id",
                            TIERS."Name" AS "Name",
                            COUNT (IDENTITIES."TierId") AS "NumberOfIdentities",
                            TIERS."CanBeUsedAsDefaultForClient" AS "CanBeUsedAsDefaultForClient",
                            TIERS."CanBeManuallyAssigned" AS "CanBeManuallyAssigned"
                        FROM "Devices"."Tiers" TIERS
                        LEFT JOIN "Devices"."Identities" IDENTITIES
                        ON IDENTITIES."TierId" = TIERS."Id"
                        GROUP BY TIERS."Id", TIERS."Name", TIERS."CanBeUsedAsDefaultForClient", TIERS."CanBeManuallyAssigned"
                 """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW "AdminUi"."MessageOverviews" AS
                        SELECT
                            "Messages"."Id" AS "MessageId",
                            "Messages"."CreatedBy" AS "SenderAddress",
                            "Messages"."CreatedByDevice" AS "SenderDevice",
                            "Messages"."CreatedAt" AS "SendDate",
                            COUNT ("Attachments"."Id") AS "NumberOfAttachments"
                        FROM
                            "Messages"."Messages" AS "Messages"
                        LEFT JOIN
                            "Messages"."Attachments" AS "Attachments"
                        ON
                            "Messages"."Id" = "Attachments"."MessageId"
                        GROUP BY
                            "Messages"."Id", "Messages"."CreatedBy", "Messages"."CreatedByDevice", "Messages"."CreatedAt"
                """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW "AdminUi"."ClientOverviews" AS
                        SELECT
                            CLIENTS."ClientId" AS "ClientId",
                            CLIENTS."DisplayName" AS "DisplayName",
                            CLIENTS."DefaultTier" AS "DefaultTierId",
                            TIERS."Name" AS "DefaultTierName",
                            CLIENTS."CreatedAt" AS "CreatedAt",
                             (
                                SELECT COUNT("ClientId") 
                                FROM "Devices"."Identities"
                                WHERE "ClientId" = CLIENTS."ClientId"
                            ) AS "NumberOfIdentities",
                             CLIENTS."MaxIdentities" AS "MaxIdentities"
                        FROM "Devices"."OpenIddictApplications" CLIENTS
                        LEFT JOIN "Devices"."Tiers" TIERS
                        ON TIERS."Id" = CLIENTS."DefaultTier"
             """);
        }
    }
}
