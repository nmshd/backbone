using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class DeletionProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalTable: "SyncRuns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalTable: "SyncRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
