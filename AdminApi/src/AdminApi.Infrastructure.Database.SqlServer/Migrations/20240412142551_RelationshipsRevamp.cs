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
                 CREATE OR REPLACE VIEW RelationshipOverviews AS
                     SELECT
                         [RELATIONSHIPS].[From] AS [From],
                         [RELATIONSHIPS].[To] AS [To],
                         [RELATIONSHIPS].[RelationshipTemplateId] AS [RelationshipTemplateId],
                         [RELATIONSHIPS].[Status] AS [Status],
                         [RELATIONSHIPS].[CreatedAt] AS [CreatedAt],
                         [AUDITLOG].[CreatedAt] AS [AnsweredAt],
                         [RELATIONSHIPS].[CreatedByDevice] AS [CreatedByDevice],
                         [AUDITLOG].[CreatedByDevice] AS [AnsweredByDevice]
                     FROM
                         [Relationships].[Relationships] AS RELATIONSHIPS
                     LEFT JOIN
                         [Relationships].[RelationshipAuditLog] AS AUDITLOG ON [RELATIONSHIPS].[Id] = [AUDITLOG].[RelationshipId]
                     WHERE // TODO (Daniel Almeida): finish the query
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 CREATE OR REPLACE VIEW RelationshipOverviews AS
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
