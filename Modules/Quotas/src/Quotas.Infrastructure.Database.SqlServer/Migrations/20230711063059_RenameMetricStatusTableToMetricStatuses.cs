using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameMetricStatusTableToMetricStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetricStatus_Identities_Owner",
                schema: "Quotas",
                table: "MetricStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatus",
                schema: "Quotas",
                table: "MetricStatus");

            migrationBuilder.RenameTable(
                name: "MetricStatus",
                schema: "Quotas",
                newName: "MetricStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatuses",
                schema: "Quotas",
                table: "MetricStatuses",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatuses_Identities_Owner",
                schema: "Quotas",
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
                schema: "Quotas",
                table: "MetricStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricStatuses",
                schema: "Quotas",
                table: "MetricStatuses");

            migrationBuilder.RenameTable(
                name: "MetricStatuses",
                schema: "Quotas",
                newName: "MetricStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricStatus",
                schema: "Quotas",
                table: "MetricStatus",
                columns: new[] { "Owner", "MetricKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_MetricStatus_Identities_Owner",
                schema: "Quotas",
                table: "MetricStatus",
                column: "Owner",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
