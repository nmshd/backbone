using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IndividualQuotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tiers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "TierQuotas",
                type: "char(36)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetricKey",
                table: "TierQuotaDefinitions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IsExhaustedUntil",
                table: "MetricStatuses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TierId",
                table: "Identities",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "IndividualQuotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    ApplyTo = table.Column<string>(type: "char(36)", nullable: false),
                    MetricKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Max = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualQuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndividualQuotas_Identities_ApplyTo",
                        column: x => x.ApplyTo,
                        principalTable: "Identities",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndividualQuotas_ApplyTo",
                table: "IndividualQuotas",
                column: "ApplyTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities");

            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas");

            migrationBuilder.DropTable(
                name: "IndividualQuotas");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tiers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "TierQuotas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "MetricKey",
                table: "TierQuotaDefinitions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IsExhaustedUntil",
                table: "MetricStatuses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "TierId",
                table: "Identities",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Identities_Tiers_TierId",
                table: "Identities",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address");
        }
    }
}
