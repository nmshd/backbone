using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddModificationsRequestedByIdentityDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Environment",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionGracePeriodEndsAt",
                schema: "Devices",
                table: "Identities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Devices",
                table: "Identities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TierIdBeforeDeletion",
                schema: "Devices",
                table: "Identities",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcesses",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovalReminder1SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovalReminder2SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovalReminder3SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    GracePeriodEndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GracePeriodReminder1SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GracePeriodReminder2SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GracePeriodReminder3SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdentityAddress = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDeletionProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                        column: x => x.IdentityAddress,
                        principalSchema: "Devices",
                        principalTable: "Identities",
                        principalColumn: "Address");
                }
            );

            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcessAuditLog",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IdentityAddressHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    DeviceIdHash = table.Column<byte[]>(type: "bytea", nullable: true),
                    OldStatus = table.Column<int>(type: "integer", nullable: true),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    IdentityDeletionProcessId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDeletionProcessAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_I~",
                        column: x => x.IdentityDeletionProcessId,
                        principalSchema: "Devices",
                        principalTable: "IdentityDeletionProcesses",
                        principalColumn: "Id");
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDeletionProcessAuditLog_IdentityDeletionProcessId",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                column: "IdentityDeletionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDeletionProcesses_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                schema: "Devices",
                name: "IdentityDeletionProcessAuditLog");

            migrationBuilder.DropTable(
                schema: "Devices",
                name: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "DeletionGracePeriodEndsAt",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropColumn(
                name: "TierIdBeforeDeletion",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.AlterColumn<int>(
                name: "Environment",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
