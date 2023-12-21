using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddGracePeriodReminderToIdentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodReminder1SentAt",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodReminder2SentAt",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodReminder3SentAt",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GracePeriodReminder1SentAt",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "GracePeriodReminder2SentAt",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "GracePeriodReminder3SentAt",
                table: "IdentityDeletionProcesses");
        }
    }
}
