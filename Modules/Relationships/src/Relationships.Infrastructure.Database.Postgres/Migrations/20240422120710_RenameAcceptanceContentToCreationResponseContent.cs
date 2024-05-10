using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RenameAcceptanceContentToCreationResponseContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcceptanceContent",
                schema: "Relationships",
                table: "Relationships",
                newName: "CreationResponseContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationResponseContent",
                schema: "Relationships",
                table: "Relationships",
                newName: "AcceptanceContent");
        }
    }
}
