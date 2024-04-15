using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Database.SqlServer.Migrations;

/// <inheritdoc />
public partial class IdentitiesBasicTier : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "TierId",
            schema: "Devices",
            table: "Identities",
            type: "char(20)",
            unicode: false,
            fixedLength: true,
            maxLength: 20,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tier_Name",
            schema: "Devices",
            table: "Tier",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Identities_TierId",
            schema: "Devices",
            table: "Identities",
            column: "TierId");

        migrationBuilder.AddForeignKey(
            name: "FK_Identities_Tier_TierId",
            schema: "Devices",
            table: "Identities",
            column: "TierId",
            principalTable: "Tier",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Identities_Tier_TierId",
            schema: "Devices",
            table: "Identities");

        migrationBuilder.DropIndex(
            name: "IX_Tier_Name",
            schema: "Devices",
            table: "Tier");

        migrationBuilder.DropIndex(
            name: "IX_Identities_TierId",
            schema: "Devices",
            table: "Identities");

        migrationBuilder.DropColumn(
            name: "TierId",
            schema: "Devices",
            table: "Identities");
    }
}
