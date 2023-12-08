using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
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

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
