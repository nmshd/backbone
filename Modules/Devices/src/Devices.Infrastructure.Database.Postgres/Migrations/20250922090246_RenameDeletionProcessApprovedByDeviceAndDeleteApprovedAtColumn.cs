using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RenameDeletionProcessApprovedByDeviceAndDeleteApprovedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.RenameColumn(
                name: "ApprovedByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                newName: "CreatedByDevice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "CreatedByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                newName: "ApprovedByDevice");
        }
    }
}
