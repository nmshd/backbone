using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRelationshipDecomposedByRecipientAndIsRelationshipDecomposedBySenderProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRelationshipDecomposedByRecipient",
                schema: "Messages",
                table: "RecipientInformation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRelationshipDecomposedBySender",
                schema: "Messages",
                table: "RecipientInformation",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRelationshipDecomposedByRecipient",
                schema: "Messages",
                table: "RecipientInformation");

            migrationBuilder.DropColumn(
                name: "IsRelationshipDecomposedBySender",
                schema: "Messages",
                table: "RecipientInformation");
        }
    }
}
