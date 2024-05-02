using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                schema: "Quotas",
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas");

            migrationBuilder.DropForeignKey(
                schema: "Quotas",
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas");

            migrationBuilder.DropForeignKey(
                schema: "Quotas",
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                schema: "Quotas",
                name: "PK_MetricStatuses",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                schema: "Quotas",
                name: "PK_Identities",
                table: "Identities");

            #region AlterColumns
            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "ApplyTo",
                table: "TierQuotas",
                type: "character(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)");

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "Owner",
                table: "MetricStatuses",
                type: "character(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)");

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "ApplyTo",
                table: "IndividualQuotas",
                type: "character(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)");

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "Address",
                table: "Identities",
                type: "character(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);

            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                schema: "Quotas",
                name: "PK_Identities",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddPrimaryKey(
                schema: "Quotas",
                name: "PK_MetricStatuses",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                schema: "Quotas",
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalSchema: "Quotas",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                schema: "Quotas",
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalSchema: "Quotas",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                schema: "Quotas",
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalSchema: "Quotas",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                schema: "Quotas",
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas");

            migrationBuilder.DropForeignKey(
                schema: "Quotas",
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas");

            migrationBuilder.DropForeignKey(
                schema: "Quotas",
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                schema: "Quotas",
                name: "PK_MetricStatuses",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                schema: "Quotas",
                name: "PK_Identities",
                table: "Identities");

            #region AlterColumns

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "ApplyTo",
                table: "TierQuotas",
                type: "character(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)");

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "Owner",
                table: "MetricStatuses",
                type: "character(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)");

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "ApplyTo",
                table: "IndividualQuotas",
                type: "character(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)");

            migrationBuilder.AlterColumn<string>(
                schema: "Quotas",
                name: "Address",
                table: "Identities",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                schema: "Quotas",
                name: "PK_Identities",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddPrimaryKey(
                schema: "Quotas",
                name: "PK_MetricStatuses",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                schema: "Quotas",
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalSchema: "Quotas",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                schema: "Quotas",
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalSchema: "Quotas",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                schema: "Quotas",
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalSchema: "Quotas",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
