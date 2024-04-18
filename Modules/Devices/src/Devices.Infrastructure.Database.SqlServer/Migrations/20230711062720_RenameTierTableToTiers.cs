using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameTierTableToTiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tier_TierId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tier",
                schema: "Devices",
                table: "Tier");

            migrationBuilder.RenameTable(
                name: "Tier",
                schema: "Devices",
                newName: "Tiers",
                newSchema: "Devices");

            migrationBuilder.RenameIndex(
                name: "IX_Tier_Name",
                schema: "Devices",
                table: "Tiers",
                newName: "IX_Tiers_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tiers",
                schema: "Devices",
                table: "Tiers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Devices",
                table: "Identities",
                column: "TierId",
                principalSchema: "Devices",
                principalTable: "Tiers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tiers",
                schema: "Devices",
                table: "Tiers");

            migrationBuilder.RenameTable(
                name: "Tiers",
                schema: "Devices",
                newName: "Tier",
                newSchema: "Devices");

            migrationBuilder.RenameIndex(
                name: "IX_Tiers_Name",
                schema: "Devices",
                table: "Tier",
                newName: "IX_Tier_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tier",
                schema: "Devices",
                table: "Tier",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tier_TierId",
                schema: "Devices",
                table: "Identities",
                column: "TierId",
                principalSchema: "Devices",
                principalTable: "Tier",
                principalColumn: "Id");
        }
    }
}
