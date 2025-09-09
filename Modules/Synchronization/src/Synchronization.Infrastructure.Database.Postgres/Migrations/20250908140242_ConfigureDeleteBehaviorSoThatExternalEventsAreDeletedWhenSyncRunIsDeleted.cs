using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureDeleteBehaviorSoThatExternalEventsAreDeletedWhenSyncRunIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.AlterColumn<string>(
                name: "DatawalletId",
                schema: "Synchronization",
                table: "DatawalletModifications",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.AlterColumn<string>(
                name: "DatawalletId",
                schema: "Synchronization",
                table: "DatawalletModifications",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
