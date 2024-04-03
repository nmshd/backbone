using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Identities_ClientId",
                table: "Identities",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_TierId",
                table: "Identities",
                column: "TierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Identities_ClientId",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Identities_TierId",
                table: "Identities");
        }
    }
}
