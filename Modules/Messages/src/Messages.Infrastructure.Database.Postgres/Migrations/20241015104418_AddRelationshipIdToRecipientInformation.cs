using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipIdToRecipientInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelationshipId",
                schema: "Messages",
                table: "RecipientInformation",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            // Fill the new RelationshipId column with the existing relationships between sender and recipient
            migrationBuilder.Sql("""
                                 UPDATE "Messages"."RecipientInformation" AS ri
                                 SET "RelationshipId" = COALESCE((
                                     SELECT "Id"
                                     FROM "Relationships"."Relationships" AS r
                                     WHERE (r."From" = m."CreatedBy" AND r."To" = ri."Address") OR (r."From" = ri."Address" AND r."To" = m."CreatedBy")
                                     ORDER BY r."CreatedAt" DESC
                                     LIMIT 1
                                 ), 'REL00000000000000000')
                                 FROM "Messages"."Messages" AS m
                                 WHERE m."Id" = ri."MessageId" AND ri."RelationshipId" = '';
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelationshipId",
                schema: "Messages",
                table: "RecipientInformation");
        }
    }
}
