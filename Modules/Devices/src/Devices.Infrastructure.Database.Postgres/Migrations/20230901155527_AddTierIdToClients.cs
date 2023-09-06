using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
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
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TierId",
                table: "OpenIddictApplications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Devices"."OpenIddictApplications"
                SET "TierId" = (SELECT "Id" FROM "Devices"."Tiers" WHERE "Name" = 'Basic')
                WHERE "TierId" IS NULL
            """);

            migrationBuilder.AlterColumn<string>(
                name: "TierId",
                table: "OpenIddictApplications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TierId",
                table: "OpenIddictApplications");

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "PnsRegistrations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
