using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Quotas");

            migrationBuilder.CreateTable(
                name: "Tiers",
                schema: "Quotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Identities",
                schema: "Quotas",
                columns: table => new
                {
                    Address = table.Column<string>(type: "character varying(80)", unicode: false, maxLength: 80, nullable: false),
                    TierId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Address);
                    table.ForeignKey(
                        name: "FK_Identities_Tiers_TierId",
                        column: x => x.TierId,
                        principalSchema: "Quotas",
                        principalTable: "Tiers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TierQuotaDefinitions",
                schema: "Quotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    MetricKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Max = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    TierId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TierQuotaDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TierQuotaDefinitions_Tiers_TierId",
                        column: x => x.TierId,
                        principalSchema: "Quotas",
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndividualQuotas",
                schema: "Quotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    ApplyTo = table.Column<string>(type: "character varying(80)", nullable: false),
                    MetricKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Max = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualQuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndividualQuotas_Identities_ApplyTo",
                        column: x => x.ApplyTo,
                        principalSchema: "Quotas",
                        principalTable: "Identities",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetricStatuses",
                schema: "Quotas",
                columns: table => new
                {
                    MetricKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Owner = table.Column<string>(type: "character varying(80)", nullable: false),
                    IsExhaustedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricStatuses", x => new { x.Owner, x.MetricKey });
                    table.ForeignKey(
                        name: "FK_MetricStatuses_Identities_Owner",
                        column: x => x.Owner,
                        principalSchema: "Quotas",
                        principalTable: "Identities",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TierQuotas",
                schema: "Quotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    DefinitionId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    ApplyTo = table.Column<string>(type: "character varying(80)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TierQuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TierQuotas_Identities_ApplyTo",
                        column: x => x.ApplyTo,
                        principalSchema: "Quotas",
                        principalTable: "Identities",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TierQuotas_TierQuotaDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "Quotas",
                        principalTable: "TierQuotaDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_TierId",
                schema: "Quotas",
                table: "Identities",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualQuotas_ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                column: "ApplyTo");

            migrationBuilder.CreateIndex(
                name: "IX_MetricStatuses_MetricKey",
                schema: "Quotas",
                table: "MetricStatuses",
                column: "MetricKey")
                .Annotation("Npgsql:IndexInclude", new[] { "IsExhaustedUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_TierQuotaDefinitions_TierId",
                schema: "Quotas",
                table: "TierQuotaDefinitions",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_TierQuotas_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                column: "ApplyTo");

            migrationBuilder.CreateIndex(
                name: "IX_TierQuotas_DefinitionId",
                schema: "Quotas",
                table: "TierQuotas",
                column: "DefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndividualQuotas",
                schema: "Quotas");

            migrationBuilder.DropTable(
                name: "MetricStatuses",
                schema: "Quotas");

            migrationBuilder.DropTable(
                name: "TierQuotas",
                schema: "Quotas");

            migrationBuilder.DropTable(
                name: "Identities",
                schema: "Quotas");

            migrationBuilder.DropTable(
                name: "TierQuotaDefinitions",
                schema: "Quotas");

            migrationBuilder.DropTable(
                name: "Tiers",
                schema: "Quotas");
        }
    }
}
