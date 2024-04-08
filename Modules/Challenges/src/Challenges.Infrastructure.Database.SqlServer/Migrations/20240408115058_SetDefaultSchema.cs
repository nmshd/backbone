using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Challenges.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Challenges");

            migrationBuilder.RenameTable(
                name: "Challenges",
                newName: "Challenges",
                newSchema: "Challenges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Challenges",
                schema: "Challenges",
                newName: "Challenges");
        }
    }
}
