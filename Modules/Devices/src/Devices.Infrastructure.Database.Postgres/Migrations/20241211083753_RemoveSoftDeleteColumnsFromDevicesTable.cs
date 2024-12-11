using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeleteColumnsFromDevicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DELETE FROM "Devices"."Devices" WHERE "DeletedAt" IS NOT NULL""");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Devices",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeletedByDevice",
                schema: "Devices",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Devices",
                table: "Devices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByDevice",
                schema: "Devices",
                table: "Devices",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);
        }
    }
}
