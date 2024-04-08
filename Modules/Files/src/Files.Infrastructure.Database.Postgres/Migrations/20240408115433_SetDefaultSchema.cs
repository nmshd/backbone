using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Files");

            migrationBuilder.RenameTable(
                name: "FileMetadata",
                newName: "FileMetadata",
                newSchema: "Files");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "FileMetadata",
                schema: "Files",
                newName: "FileMetadata");
        }
    }
}
