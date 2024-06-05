using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessRejectionProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "RejectedByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses");
        }
    }
}
