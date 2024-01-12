using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Reminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalReminder1SentAt",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalReminder2SentAt",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalReminder3SentAt",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalReminder1SentAt",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "ApprovalReminder2SentAt",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "ApprovalReminder3SentAt",
                table: "IdentityDeletionProcesses");
        }
    }
}
