using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileMetadata_CreatedBy",
                schema: "Files",
                table: "FileMetadata");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_CreatedBy",
                schema: "Files",
                table: "FileMetadata",
                column: "CreatedBy");
        }
    }
}
