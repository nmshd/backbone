using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddModificationsRequestedByCancelDeletionProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByDevice",
                table: "IdentityDeletionProcesses",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "CancelledByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses");
        }
    }
}
