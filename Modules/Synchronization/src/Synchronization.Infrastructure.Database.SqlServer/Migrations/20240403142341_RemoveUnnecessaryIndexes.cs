using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SyncRuns_CreatedBy",
                table: "SyncRuns");

            migrationBuilder.DropIndex(
                name: "IX_DatawalletModifications_CreatedBy",
                table: "DatawalletModifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SyncRuns_CreatedBy",
                table: "SyncRuns",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DatawalletModifications_CreatedBy",
                table: "DatawalletModifications",
                column: "CreatedBy");
        }
    }
}
