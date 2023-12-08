using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class DeletionProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionStartedAt",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Identities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "DeletionStartedAt",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Identities");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address");
        }
    }
}
