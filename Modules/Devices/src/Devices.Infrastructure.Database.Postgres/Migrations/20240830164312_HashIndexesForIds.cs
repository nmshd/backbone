using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    [DependsOn(ModuleType.Devices, "20240708114348_AddAdditionalDataToIdentityDeletionProcessAuditLogEntry")]
    public partial class HashIndexesForIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Identities_ClientId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Identities_TierId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_ClientId",
                schema: "Devices",
                table: "Identities",
                column: "ClientId")
                .Annotation("Npgsql:IndexMethod", "hash");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_TierId",
                schema: "Devices",
                table: "Identities",
                column: "TierId")
                .Annotation("Npgsql:IndexMethod", "hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Identities_ClientId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Identities_TierId",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_ClientId",
                schema: "Devices",
                table: "Identities",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_TierId",
                schema: "Devices",
                table: "Identities",
                column: "TierId");
        }
    }
}
