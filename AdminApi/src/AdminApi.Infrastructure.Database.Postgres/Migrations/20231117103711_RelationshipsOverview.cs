using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminUi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO
                $$
                BEGIN
                IF EXISTS (
                   SELECT FROM information_schema.tables 
                   WHERE  table_schema = 'Relationships'
                   AND    table_name = 'RelationshipChanges'
                ) THEN
                   CREATE VIEW "RelationshipOverviews" AS
        		        SELECT
        			        "Relationships"."From" AS "From",
        			        "Relationships"."To" AS "To",
        			        "Relationships"."RelationshipTemplateId" AS "RelationshipTemplateId",
        			        "Relationships"."Status" AS "Status",
        			        "Relationships"."CreatedAt" AS "CreatedAt",
        			        "RelationshipChanges"."Res_CreatedAt" AS "AnsweredAt",
        			        "RelationshipChanges"."Req_CreatedByDevice" AS "CreatedByDevice",
        			        "RelationshipChanges"."Res_CreatedByDevice" AS "AnsweredByDevice"
        		        FROM
        			        "Relationships"."Relationships" AS "Relationships"
        		        LEFT JOIN
        			        "Relationships"."RelationshipChanges" AS "RelationshipChanges"
        		        ON
        			        "Relationships"."Id" = "RelationshipChanges"."RelationshipId"
        		        WHERE
        			        "RelationshipChanges"."Type" = 10;
                END IF;
                END;
                $$
                
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW "RelationshipOverviews" """);
        }
    }
}
