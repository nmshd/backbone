using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IndividualQuotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Quotas",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Quotas",
                table: "Tiers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                type: "character(36)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetricKey",
                schema: "Quotas",
                table: "TierQuotaDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IsExhaustedUntil",
                schema: "Quotas",
                table: "MetricStatuses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TierId",
                schema: "Quotas",
                table: "Identities",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "IndividualQuotas",
                schema: "Quotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    ApplyTo = table.Column<string>(type: "character(36)", nullable: false),
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
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_IndividualQuotas_ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                column: "ApplyTo");

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
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                column: "ApplyTo",
                principalSchema: "Quotas",
                principalTable: "Identities",
                principalColumn: "Address",
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
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.DropTable(
                schema: "Quotas",
                name: "IndividualQuotas");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Quotas",
                table: "Tiers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                type: "character(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(36)");

            migrationBuilder.AlterColumn<string>(
                name: "MetricKey",
                schema: "Quotas",
                table: "TierQuotaDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IsExhaustedUntil",
                schema: "Quotas",
                table: "MetricStatuses",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "TierId",
                schema: "Quotas",
                table: "Identities",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                schema: "Quotas",
                table: "Identities",
                column: "TierId",
                principalSchema: "Quotas",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                column: "ApplyTo",
                principalSchema: "Quotas",
                principalTable: "Identities",
                principalColumn: "Address");
        }
    }
}
