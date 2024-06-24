using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddresses_Varying_Length : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                type: "character varying(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                schema: "Quotas",
                table: "MetricStatuses",
                type: "character varying(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                type: "character varying(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Quotas",
                table: "Identities",
                type: "character varying(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);

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

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "TierQuotas",
                type: "character(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                schema: "Quotas",
                table: "MetricStatuses",
                type: "character(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                schema: "Quotas",
                table: "IndividualQuotas",
                type: "character(80)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Quotas",
                table: "Identities",
                type: "character(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldUnicode: false,
                oldMaxLength: 80);

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
