using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class DeletionProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications");

            migrationBuilder.AddForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications",
                column: "DatawalletId",
                principalTable: "Datawallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications");

            migrationBuilder.AddForeignKey(
                name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                table: "DatawalletModifications",
                column: "DatawalletId",
                principalTable: "Datawallets",
                principalColumn: "Id");
        }
    }
}
