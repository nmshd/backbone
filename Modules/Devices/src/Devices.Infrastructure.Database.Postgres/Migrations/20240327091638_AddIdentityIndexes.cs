using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Identities_ClientId",
                schema: "Devices",
                table: "Identities",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_TierId",
                schema: "Devices",
                table: "Identities",
                column: "TierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Identities_ClientId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Identities_TierId",
                schema: "Devices",
                table: "Identities");
        }
    }
}
