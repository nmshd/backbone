using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOpenIddictToV5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "Devices",
                table: "OpenIddictApplications",
                newName: "ClientType");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationType",
                schema: "Devices",
                table: "OpenIddictApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JsonWebKeySet",
                schema: "Devices",
                table: "OpenIddictApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                schema: "Devices",
                table: "OpenIddictApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationType",
                schema: "Devices",
                table: "OpenIddictApplications");

            migrationBuilder.DropColumn(
                name: "JsonWebKeySet",
                schema: "Devices",
                table: "OpenIddictApplications");

            migrationBuilder.DropColumn(
                name: "Settings",
                schema: "Devices",
                table: "OpenIddictApplications");

            migrationBuilder.RenameColumn(
                name: "ClientType",
                schema: "Devices",
                table: "OpenIddictApplications",
                newName: "Type");
        }
    }
}
