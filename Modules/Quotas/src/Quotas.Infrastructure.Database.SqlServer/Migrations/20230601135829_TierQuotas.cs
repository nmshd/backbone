using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class TierQuotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TierQuotaDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    MetricKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Max = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false),
                    TierId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TierQuotaDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TierQuotaDefinitions_Tiers_TierId",
                        column: x => x.TierId,
                        principalTable: "Tiers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TierQuotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    IdentityAddress = table.Column<string>(type: "char(36)", nullable: true),
                    DefinitionId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    IsExhaustedUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TierQuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TierQuotas_Identities_IdentityAddress",
                        column: x => x.IdentityAddress,
                        principalTable: "Identities",
                        principalColumn: "Address");
                    table.ForeignKey(
                        name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "TierQuotaDefinitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TierQuotaDefinitions_TierId",
                table: "TierQuotaDefinitions",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_TierQuotas_DefinitionId",
                table: "TierQuotas",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TierQuotas_IdentityAddress",
                table: "TierQuotas",
                column: "IdentityAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TierQuotas");

            migrationBuilder.DropTable(
                name: "TierQuotaDefinitions");
        }
    }
}
