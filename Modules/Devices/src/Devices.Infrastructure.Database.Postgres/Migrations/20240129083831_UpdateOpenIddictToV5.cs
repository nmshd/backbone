using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
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
                table: "OpenIddictApplications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JsonWebKeySet",
                table: "OpenIddictApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "OpenIddictApplications",
                type: "text",
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
