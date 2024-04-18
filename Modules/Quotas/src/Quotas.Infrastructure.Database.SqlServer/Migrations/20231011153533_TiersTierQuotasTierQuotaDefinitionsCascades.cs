using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class TiersTierQuotasTierQuotaDefinitionsCascades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Quotas",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Quotas",
                table: "Identities",
                column: "TierId",
                principalSchema: "Quotas",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions",
                column: "TierId",
                principalSchema: "Quotas",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                schema: "Quotas",
                table: "TierQuotas",
                column: "DefinitionId",
                principalSchema: "Quotas",
                principalTable: "TierQuotaDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Quotas",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Quotas",
                table: "Identities",
                column: "TierId",
                principalSchema: "Quotas",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotaDefinitions_Tiers_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions",
                column: "TierId",
                principalSchema: "Quotas",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                schema: "Quotas",
                table: "TierQuotas",
                column: "DefinitionId",
                principalSchema: "Quotas",
                principalTable: "TierQuotaDefinitions",
                principalColumn: "Id");
        }
    }
}
