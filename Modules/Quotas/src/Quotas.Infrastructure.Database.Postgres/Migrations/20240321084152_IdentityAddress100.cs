using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "TierQuotas",
                type: "character(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "MetricStatuses",
                type: "character(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "IndividualQuotas",
                type: "character(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identities",
                type: "character(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "TierQuotas",
                type: "character(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "MetricStatuses",
                type: "character(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyTo",
                table: "IndividualQuotas",
                type: "character(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identities",
                type: "character(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);
        }
    }
}
