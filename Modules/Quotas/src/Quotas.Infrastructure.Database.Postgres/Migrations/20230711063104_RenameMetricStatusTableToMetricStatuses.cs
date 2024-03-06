using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RenameMetricStatusTableToMetricStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatus_Identities_Owner",
                table: "MetricStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatus",
                table: "MetricStatus");

            migrationBuilder.RenameTable(
                name: "MetricStatus",
                newName: "MetricStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatuses",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatuses",
                table: "MetricStatuses");

            migrationBuilder.RenameTable(
                name: "MetricStatuses",
                newName: "MetricStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatus",
                table: "MetricStatus",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatus_Identities_Owner",
                table: "MetricStatus",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
