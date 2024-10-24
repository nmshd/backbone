using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeliveryBlockedAndContextColumnsToExternalEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Context",
                schema: "Synchronization",
                table: "ExternalEvents",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryBlocked",
                schema: "Synchronization",
                table: "ExternalEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Context",
                schema: "Synchronization",
                table: "ExternalEvents");

            migrationBuilder.DropColumn(
                name: "IsDeliveryBlocked",
                schema: "Synchronization",
                table: "ExternalEvents");
        }
    }
}
