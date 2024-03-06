using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class TiersTierQuotasTierQuotaDefinitionsCascades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                table: "TierQuotaDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                table: "TierQuotas");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                table: "TierQuotaDefinitions",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                table: "TierQuotas",
                column: "DefinitionId",
                principalTable: "TierQuotaDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                table: "TierQuotaDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                table: "TierQuotas");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                table: "TierQuotaDefinitions",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                table: "TierQuotas",
                column: "DefinitionId",
                principalTable: "TierQuotaDefinitions",
                principalColumn: "Id");
        }
    }
}
