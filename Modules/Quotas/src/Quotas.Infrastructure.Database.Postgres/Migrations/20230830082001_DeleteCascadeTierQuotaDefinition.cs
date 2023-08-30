using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCascadeTierQuotaDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                table: "TierQuotaDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions",
                column: "TierId",
                principalSchema: "Quotas",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                table: "TierQuotaDefinitions",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id");
        }
    }
}
