using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureDeleteBehaviorSoThatSyncRunIdOnSyncErrorsIsSetToNullSyncRunIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.DropIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.AlterColumn<string>(
                name: "SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
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

            migrationBuilder.CreateIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors",
                columns: new[] { "SyncRunId", "ExternalEventId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.DropIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.AlterColumn<string>(
                name: "SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
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

            migrationBuilder.CreateIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors",
                columns: new[] { "SyncRunId", "ExternalEventId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id");
        }
    }
}
