using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RenameTierTableToTiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tier_TierId",
                table: "Identities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tier",
                table: "Tier");

            migrationBuilder.RenameTable(
                name: "Tier",
                newName: "Tiers");

            migrationBuilder.RenameIndex(
                name: "IX_Tier_Name",
                table: "Tiers",
                newName: "IX_Tiers_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tiers",
                table: "Tiers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tiers",
                table: "Tiers");

            migrationBuilder.RenameTable(
                name: "Tiers",
                newName: "Tier");

            migrationBuilder.RenameIndex(
                name: "IX_Tiers_Name",
                table: "Tier",
                newName: "IX_Tier_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tier",
                table: "Tier",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tier_TierId",
                table: "Identities",
                column: "TierId",
                principalTable: "Tier",
                principalColumn: "Id");
        }
    }
}
