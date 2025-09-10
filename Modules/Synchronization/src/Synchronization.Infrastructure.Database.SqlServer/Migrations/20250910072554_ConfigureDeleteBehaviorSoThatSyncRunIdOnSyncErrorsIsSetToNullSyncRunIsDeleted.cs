using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
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
