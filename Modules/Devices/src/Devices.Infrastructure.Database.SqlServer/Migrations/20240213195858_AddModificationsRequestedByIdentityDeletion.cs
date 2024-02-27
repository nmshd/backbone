using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddModificationsRequestedByIdentityDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Environment",
                table: "PnsRegistrations",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Identities",
                type: "int",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.AddColumn<string>(
                name: "TierIdBeforeDeletion",
                table: "Identities",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcesses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalReminder1SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalReminder2SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalReminder3SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    GracePeriodEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GracePeriodReminder1SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GracePeriodReminder2SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GracePeriodReminder3SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    IdentityAddress = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDeletionProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                        column: x => x.IdentityAddress,
                        principalTable: "Identities",
                        principalColumn: "Address");
                });

            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcessAuditLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityAddressHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DeviceIdHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    OldStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    IdentityDeletionProcessId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDeletionProcessAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_IdentityDeletionProcessId",
                        column: x => x.IdentityDeletionProcessId,
                        principalTable: "IdentityDeletionProcesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDeletionProcessAuditLog_IdentityDeletionProcessId",
                table: "IdentityDeletionProcessAuditLog",
                column: "IdentityDeletionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDeletionProcesses_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityDeletionProcessAuditLog");

            migrationBuilder.DropTable(
                name: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "TierIdBeforeDeletion",
                table: "Identities");

            migrationBuilder.AlterColumn<int>(
                name: "Environment",
                table: "PnsRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
