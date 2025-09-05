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

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id");
        }
    }
}
