using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.SqlServer.Migrations
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
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                 UPDATE ri
                                 SET ri.RelationshipId = COALESCE((
                                     SELECT TOP 1 r.Id 
                                     FROM Relationships.Relationships AS r
                                     WHERE (r.[From] = m.CreatedBy AND r.[To] = ri.Address) OR (r.[From] = ri.Address AND r.[To] = m.CreatedBy)
                                     ORDER BY r.CreatedAt DESC
                                 ), 'REL00000000000000000')
                                 FROM Messages.RecipientInformation AS ri
                                 JOIN Messages.Messages AS m ON m.Id = ri.MessageId
                                 WHERE ri.RelationshipId = '';
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
