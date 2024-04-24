using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddBlobReferencePropertyToDatawalletModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobReference",
                schema: "Synchronization",
                table: "DatawalletModifications",
                type: "character(32)",
                unicode: false,
                fixedLength: true,
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobReference",
                schema: "Synchronization",
                table: "DatawalletModifications");
        }
    }
}
