using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTierProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanBeManuallyAssigned",
                schema: "Devices",
                table: "Tiers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData("Tiers", "Id", "TIR00000000000000001", "CanBeManuallyAssigned", false);

            migrationBuilder.AddColumn<bool>(
                name: "CanBeUsedAsDefaultForClient",
                schema: "Devices",
                table: "Tiers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData("Tiers", "Id", "TIR00000000000000001", "CanBeUsedAsDefaultForClient", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanBeManuallyAssigned",
                schema: "Devices",
                table: "Tiers");

            migrationBuilder.DropColumn(
                name: "CanBeUsedAsDefaultForClient",
                schema: "Devices",
                table: "Tiers");
        }
    }
}
