using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSnapshotToReferToAllTypesOfModulesForExport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [AdminUi].[IdentityOverviews]");
            migrationBuilder.Sql("DROP VIEW [AdminUi].[RelationshipOverviews]");
            migrationBuilder.Sql("DROP VIEW [AdminUi].[TierOverviews]");
            migrationBuilder.Sql("DROP VIEW [AdminUi].[MessageOverviews]");
            migrationBuilder.Sql("DROP VIEW [AdminUi].[ClientOverviews]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                    CREATE VIEW [AdminUi].[IdentityOverviews] AS
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
                                DEVICES.IdentityAddress,
                                MAX(USERS.LastLoginAt) AS LastLoginAt
                            FROM Devices.AspNetUsers USERS
                            INNER JOIN Devices.Devices DEVICES ON DEVICES.Id = USERS.DeviceId
                            GROUP BY DEVICES.IdentityAddress
                        ) AS USERS ON USERS.IdentityAddress = IDENTITIES.Address
                        LEFT JOIN Synchronization.Datawallets DATAWALLETS ON DATAWALLETS.Owner = IDENTITIES.Address
                        LEFT JOIN Devices.Tiers TIERS ON TIERS.Id = IDENTITIES.TierId
                """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW [AdminUi].[RelationshipOverviews] AS
                        SELECT
                            [RELATIONSHIPS].[From] AS [From],
                            [RELATIONSHIPS].[To] AS [To],
                            [RELATIONSHIPS].[RelationshipTemplateId] AS [RelationshipTemplateId],
                            [RELATIONSHIPS].[Status] AS [Status],
                            [AUDITLOG1].[CreatedAt] AS [CreatedAt],
                            [AUDITLOG1].[CreatedByDevice] AS [CreatedByDevice],
                            [AUDITLOG2].[CreatedAt] AS [AnsweredAt],
                            [AUDITLOG2].[CreatedByDevice] AS [AnsweredByDevice]
                        FROM
                            [Relationships].[Relationships] AS RELATIONSHIPS
                        LEFT JOIN 
                            [Relationships].[RelationshipAuditLog] AS AUDITLOG1 
                        ON 
                            [RELATIONSHIPS].[Id] = [AUDITLOG1].[RelationshipId] AND [AUDITLOG1].[Reason] = 0
                        LEFT JOIN 
                            [Relationships].[RelationshipAuditLog] AS AUDITLOG2 
                        ON 
                            [RELATIONSHIPS].[Id] = [AUDITLOG2].[RelationshipId] AND [AUDITLOG2].[Reason] = 1
                """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW AdminUi.TierOverviews AS
                        SELECT
                            TIERS.Id,
                            TIERS.Name,
                            COUNT (IDENTITIES.TierId) AS NumberOfIdentities,
                            TIERS.CanBeUsedAsDefaultForClient,
                            TIERS.CanBeManuallyAssigned
                        FROM Devices.Tiers TIERS
                        LEFT JOIN Devices.Identities IDENTITIES
                        ON IDENTITIES.TierId = TIERS.Id
                        GROUP BY TIERS.Id, TIERS.Name, TIERS.CanBeUsedAsDefaultForClient, TIERS.CanBeManuallyAssigned
                """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW [AdminUi].[MessageOverviews] AS
                        SELECT
                            [MESSAGES].[Id] AS [MessageId],
                            [MESSAGES].[CreatedBy] AS [SenderAddress],
                            [MESSAGES].[CreatedByDevice] AS [SenderDevice],
                            [MESSAGES].[CreatedAt] AS [SendDate],
                            COUNT ([Attachments].[Id]) AS [NumberOfAttachments]
                        FROM
                            [Messages].[Messages] AS MESSAGES
                        LEFT JOIN
                            [Messages].[Attachments] AS ATTACHMENTS
                        ON
                            [MESSAGES].[Id] = [ATTACHMENTS].[MessageId]
                        GROUP BY
                            [MESSAGES].[Id], [MESSAGES].[CreatedBy], [MESSAGES].[CreatedByDevice], [MESSAGES].[CreatedAt]
                """);

            migrationBuilder.Sql(
                """
                    CREATE VIEW [AdminUi].[ClientOverviews] AS
                        SELECT
                            CLIENTS.ClientId,
                            CLIENTS.DisplayName,
                            CLIENTS.DefaultTier AS DefaultTierId,
                            TIERS.Name AS DefaultTierName,
                            CLIENTS.CreatedAt,
                            CLIENTS.MaxIdentities,
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
    }
}
