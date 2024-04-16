using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT * FROM information_schema.tables WHERE  table_schema = 'Relationships' AND table_name = 'RelationshipChanges')
                BEGIN
                   EXECUTE('CREATE OR ALTER VIEW RelationshipOverviews AS
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
                           [RELATIONSHIPCHANGES].[Type] = 10')
                END
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW RelationshipOverviews """);
        }
    }
}
