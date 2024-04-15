using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ActualDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_IdentityDeletionProcessId",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionStartedAt",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_IdentityDeletionProcessId",
                table: "IdentityDeletionProcessAuditLog",
                column: "IdentityDeletionProcessId",
                principalTable: "IdentityDeletionProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_IdentityDeletionProcessId",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "DeletionStartedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_IdentityDeletionProcessId",
                table: "IdentityDeletionProcessAuditLog",
                column: "IdentityDeletionProcessId",
                principalTable: "IdentityDeletionProcesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address");
        }
    }
}
