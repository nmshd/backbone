using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsRevamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 CREATE OR ALTER VIEW [AdminUi].[RelationshipOverviews] AS
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 CREATE OR ALTER VIEW [AdminUi].[RelationshipOverviews] AS
                     SELECT
                         [RELATIONSHIPS].[From] AS [From],
                         [RELATIONSHIPS].[To] AS [To],
                         [RELATIONSHIPS].[RelationshipTemplateId] AS [RelationshipTemplateId],
                         [RELATIONSHIPS].[Status] AS [Status],
                         [RELATIONSHIPS].[CreatedAt] AS [CreatedAt],
                         [RELATIONSHIPCHANGES].[Res_CreatedAt] AS [AnsweredAt],
                         [RELATIONSHIPCHANGES].[Req_CreatedByDevice] AS [CreatedByDevice],
                         [RELATIONSHIPCHANGES].[Res_CreatedByDevice] AS [AnsweredByDevice]
                     FROM
                         [Relationships].[Relationships] AS RELATIONSHIPS
                     LEFT JOIN
                         [Relationships].[RelationshipChanges] AS RELATIONSHIPCHANGES
                     ON
                         [RELATIONSHIPS].[Id] = [RELATIONSHIPCHANGES].[RelationshipId]
                     WHERE
                         [RELATIONSHIPCHANGES].[Type] = 10
            """);
        }
    }
}
