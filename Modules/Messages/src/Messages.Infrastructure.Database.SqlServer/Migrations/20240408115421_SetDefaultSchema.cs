using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Messages");

            migrationBuilder.RenameTable(
                name: "RecipientInformation",
                newName: "RecipientInformation",
                newSchema: "Messages");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Messages",
                newSchema: "Messages");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachments",
                newSchema: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "RecipientInformation",
                schema: "Messages",
                newName: "RecipientInformation");

            migrationBuilder.RenameTable(
                name: "Messages",
                schema: "Messages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "Attachments",
                schema: "Messages",
                newName: "Attachments");
        }
    }
}
