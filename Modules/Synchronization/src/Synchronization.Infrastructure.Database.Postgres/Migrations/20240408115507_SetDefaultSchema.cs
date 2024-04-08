using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Synchronization");

            migrationBuilder.RenameTable(
                name: "SyncRuns",
                newName: "SyncRuns",
                newSchema: "Synchronization");

            migrationBuilder.RenameTable(
                name: "SyncErrors",
                newName: "SyncErrors",
                newSchema: "Synchronization");

            migrationBuilder.RenameTable(
                name: "ExternalEvents",
                newName: "ExternalEvents",
                newSchema: "Synchronization");

            migrationBuilder.RenameTable(
                name: "Datawallets",
                newName: "Datawallets",
                newSchema: "Synchronization");

            migrationBuilder.RenameTable(
                name: "DatawalletModifications",
                newName: "DatawalletModifications",
                newSchema: "Synchronization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SyncRuns",
                schema: "Synchronization",
                newName: "SyncRuns");

            migrationBuilder.RenameTable(
                name: "SyncErrors",
                schema: "Synchronization",
                newName: "SyncErrors");

            migrationBuilder.RenameTable(
                name: "ExternalEvents",
                schema: "Synchronization",
                newName: "ExternalEvents");

            migrationBuilder.RenameTable(
                name: "Datawallets",
                schema: "Synchronization",
                newName: "Datawallets");

            migrationBuilder.RenameTable(
                name: "DatawalletModifications",
                schema: "Synchronization",
                newName: "DatawalletModifications");
        }
    }
}
