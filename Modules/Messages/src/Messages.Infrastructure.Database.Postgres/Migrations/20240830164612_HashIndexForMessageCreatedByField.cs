using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class HashIndexForMessageCreatedByField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedBy",
                schema: "Messages",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedBy",
                schema: "Messages",
                table: "Messages",
                column: "CreatedBy")
                .Annotation("Npgsql:IndexMethod", "hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedBy",
                schema: "Messages",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedBy",
                schema: "Messages",
                table: "Messages",
                column: "CreatedBy");
        }
    }
}
