using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMetricStatusIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MetricStatuses_MetricKey",
                schema: "Quotas",
                table: "MetricStatuses",
                column: "MetricKey")
                .Annotation("SqlServer:Include", new[] { "IsExhaustedUntil" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MetricStatuses_MetricKey",
                schema: "Quotas",
                table: "MetricStatuses");
        }
    }
}
