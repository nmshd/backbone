using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletionProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionGracePeriodEndsAt",
                table: "Identities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcesses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    GracePeriodEndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdentityAddress = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true)
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
                name: "DeletionGracePeriodEndsAt",
                table: "Identities");
        }
    }
}
