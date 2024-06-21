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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
