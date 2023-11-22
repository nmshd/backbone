using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class CascadeIdentityDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications",
                column: "DatawalletId",
                principalTable: "Datawallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalTable: "SyncRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications",
                column: "DatawalletId",
                principalTable: "Datawallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                table: "ExternalEvents",
                column: "SyncRunId",
                principalTable: "SyncRuns",
                principalColumn: "Id");
        }
    }
}
