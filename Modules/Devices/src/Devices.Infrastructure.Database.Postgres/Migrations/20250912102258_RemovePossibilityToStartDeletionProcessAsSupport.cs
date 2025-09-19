using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemovePossibilityToStartDeletionProcessAsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalReminder1SentAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "ApprovalReminder2SentAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "ApprovalReminder3SentAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "CancelledByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "RejectedByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses");
            
            migrationBuilder.Sql(
                """
                DELETE FROM "Devices"."IdentityDeletionProcesses" WHERE "Status" NOT IN (1, 2, 10)
                """);
            
            migrationBuilder.Sql(
                """
                DELETE FROM "Devices"."IdentityDeletionProcessAuditLog" 
                WHERE "MessageKey" IN ('StartedBySupport', 'Approved', 'Rejected', 'CancelledBySupport', 'CancelledAutomatically', 'ApprovalReminder1Sent', 'ApprovalReminder2Sent', 'ApprovalReminder3Sent')
                    OR OldStatus NOT IN (1, 2, 10) 
                    OR NewStatus NOT IN (1, 2, 10)
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalReminder1SentAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalReminder2SentAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalReminder3SentAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByDevice",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);
        }
    }
}
