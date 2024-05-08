using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Tokens.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_CreatedBy",
                schema: "Tokens",
                table: "Tokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tokens_CreatedBy",
                schema: "Tokens",
                table: "Tokens",
                column: "CreatedBy");
        }
    }
}
