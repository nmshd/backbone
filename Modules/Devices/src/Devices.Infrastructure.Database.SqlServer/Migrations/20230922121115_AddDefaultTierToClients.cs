using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultTierToClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultTier",
                table: "OpenIddictApplications",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictApplications_DefaultTier",
                schema: "Devices",
                table: "OpenIddictApplications",
                column: "DefaultTier");

            migrationBuilder.Sql(@"
                UPDATE [Devices].[OpenIddictApplications]
                SET DefaultTier = (
	                SELECT Id
	                FROM [Devices].[Tiers]
	                WHERE Name = 'Basic'
                )
                WHERE DefaultTier = ''
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_OpenIddictApplications_Tiers_DefaultTier",
                table: "OpenIddictApplications",
                column: "DefaultTier",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpenIddictApplications_Tiers_DefaultTier",
                schema: "Devices",
                table: "OpenIddictApplications");

            migrationBuilder.DropIndex(
                name: "IX_OpenIddictApplications_DefaultTier",
                schema: "Devices",
                table: "OpenIddictApplications");

            migrationBuilder.DropColumn(
                name: "DefaultTier",
                schema: "Devices",
                table: "OpenIddictApplications");

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
