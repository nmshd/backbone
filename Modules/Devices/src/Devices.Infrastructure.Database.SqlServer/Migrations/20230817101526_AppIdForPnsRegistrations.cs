using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations;

/// <inheritdoc />
public partial class AppIdForPnsRegistrations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM [Devices].[PnsRegistrations]");

        migrationBuilder.DropForeignKey(
            name: "FK_Identities_Tiers_TierId",
            table: "Identities");

        migrationBuilder.DropIndex(
            name: "IX_Identities_TierId",
            table: "Identities");

        migrationBuilder.AddColumn<string>(
            name: "AppId",
            table: "PnsRegistrations",
            type: "nvarchar(max)",
            nullable: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AppId",
            table: "PnsRegistrations");

        migrationBuilder.CreateIndex(
            name: "IX_Identities_TierId",
            schema: "Devices"
            table: "Identities",
            column: "TierId");

        migrationBuilder.AddForeignKey(
            name: "FK_Identities_Tiers_TierId",
            table: "Identities",
            column: "TierId",
            principalTable: "Tiers",
            principalColumn: "Id");
    }
}
