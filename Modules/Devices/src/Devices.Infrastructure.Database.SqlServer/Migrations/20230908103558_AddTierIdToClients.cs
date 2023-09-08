using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTierIdToClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppId",
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
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE [Devices].[OpenIddictApplications]
                SET DefaultTier = (
	                SELECT Id
	                FROM [Devices].[Tiers]
	                WHERE Name = 'Basic'
                )
                WHERE DefaultTier = NULL
            ");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultTier",
                table: "OpenIddictApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultTier",
                table: "OpenIddictApplications");

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "PnsRegistrations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
