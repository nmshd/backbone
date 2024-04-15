using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminApi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsRevamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 CREATE OR REPLACE VIEW "RelationshipOverviews" AS
                     SELECT
                         "Relationships"."From" AS "From",
                         "Relationships"."To" AS "To",
                         "Relationships"."RelationshipTemplateId" AS "RelationshipTemplateId",
                         "Relationships"."Status" AS "Status",
                         "Relationships"."CreatedAt" AS "CreatedAt",
                         "AuditLog"."CreatedAt" AS "AnsweredAt",
                         "Relationships"."CreatedByDevice" AS "CreatedByDevice",
                         "AuditLog"."CreatedByDevice" AS "AnsweredByDevice"
                     FROM "Relationships"."Relationships" AS "Relationships"
                     LEFT JOIN "Relationships"."RelationshipAuditLog" AS "AuditLog" ON "Relationships"."Id" = "AuditLog"."RelationshipId"
                     WHERE // TODO (Daniel Almeida): finish the query
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 CREATE OR REPLACE VIEW "RelationshipOverviews" AS
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
                         "RelationshipChanges"."Type" = 10
            """);
        }
    }
}
