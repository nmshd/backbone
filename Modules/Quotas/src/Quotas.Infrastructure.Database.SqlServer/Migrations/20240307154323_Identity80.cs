using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Identity100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                schema: "Quotas",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatuses",
                schema: "Quotas",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                schema: "Quotas",
                table: "Identities");

            #region AlterColumns
            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                type: "char(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                schema: "Quotas",
                table: "MetricStatuses",
                type: "char(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                type: "char(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Quotas",
                table: "Identities",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);
            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                name: "PK_Identities",
                schema: "Quotas",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatuses",
                schema: "Quotas",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                schema: "Quotas",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                schema: "Quotas",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatuses",
                schema: "Quotas",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                schema: "Quotas",
                table: "Identities");

            #region AlterColumns
            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                schema: "Quotas",
                table: "MetricStatuses",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Quotas",
                table: "Identities",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);
            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                name: "PK_Identities",
                schema: "Quotas",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatuses",
                schema: "Quotas",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                schema: "Quotas",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
