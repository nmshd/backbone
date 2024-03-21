using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class identity100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatuses",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                table: "Identities");

            #region AlterColumns
            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "TierQuotas",
                type: "char(102)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "MetricStatuses",
                type: "char(102)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "IndividualQuotas",
                type: "char(102)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identities",
                type: "char(102)",
                unicode: false,
                fixedLength: true,
                maxLength: 102,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);
            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                name: "PK_Identities",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatuses",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
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
                table: "TierQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatuses",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                table: "Identities");

            #region AlterColumns
            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "TierQuotas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(102)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "MetricStatuses",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(102)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "IndividualQuotas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(102)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identities",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(102)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 102);
            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                           name: "PK_Identities",
                           table: "Identities",
                           column: "Address");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatuses",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_TierQuotas_Identities_ApplyTo",
                table: "TierQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualQuotas_Identities_ApplyTo",
                table: "IndividualQuotas",
                column: "ApplyTo",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
