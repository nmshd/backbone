using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBlobReferenceColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobReference",
                schema: "Synchronization",
                table: "DatawalletModifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobReference",
                schema: "Synchronization",
                table: "DatawalletModifications",
                type: "char(32)",
                unicode: false,
                fixedLength: true,
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }
    }
}
