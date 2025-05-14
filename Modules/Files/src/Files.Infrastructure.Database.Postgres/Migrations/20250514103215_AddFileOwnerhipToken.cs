using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddFileOwnerhipToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FileOwnershipIsLocked",
                schema: "Files",
                table: "FileMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipToken",
                schema: "Files",
                table: "FileMetadata",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileOwnershipIsLocked",
                schema: "Files",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "OwnershipToken",
                schema: "Files",
                table: "FileMetadata");
        }
    }
}
