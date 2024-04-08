using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Quotas");

            migrationBuilder.RenameTable(
                name: "Tiers",
                newName: "Tiers",
                newSchema: "Quotas");

            migrationBuilder.RenameTable(
                name: "TierQuotas",
                newName: "TierQuotas",
                newSchema: "Quotas");

            migrationBuilder.RenameTable(
                name: "TierQuotaDefinitions",
                newName: "TierQuotaDefinitions",
                newSchema: "Quotas");

            migrationBuilder.RenameTable(
                name: "MetricStatuses",
                newName: "MetricStatuses",
                newSchema: "Quotas");

            migrationBuilder.RenameTable(
                name: "IndividualQuotas",
                newName: "IndividualQuotas",
                newSchema: "Quotas");

            migrationBuilder.RenameTable(
                name: "Identities",
                newName: "Identities",
                newSchema: "Quotas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Tiers",
                schema: "Quotas",
                newName: "Tiers");

            migrationBuilder.RenameTable(
                name: "TierQuotas",
                schema: "Quotas",
                newName: "TierQuotas");

            migrationBuilder.RenameTable(
                name: "TierQuotaDefinitions",
                schema: "Quotas",
                newName: "TierQuotaDefinitions");

            migrationBuilder.RenameTable(
                name: "MetricStatuses",
                schema: "Quotas",
                newName: "MetricStatuses");

            migrationBuilder.RenameTable(
                name: "IndividualQuotas",
                schema: "Quotas",
                newName: "IndividualQuotas");

            migrationBuilder.RenameTable(
                name: "Identities",
                schema: "Quotas",
                newName: "Identities");
        }
    }
}
