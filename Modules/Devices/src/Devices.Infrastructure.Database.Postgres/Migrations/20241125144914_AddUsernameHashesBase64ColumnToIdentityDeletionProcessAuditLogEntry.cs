using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameHashesBase64ColumnToIdentityDeletionProcessAuditLogEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NewStatus",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "UsernameHashesBase64",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsernameHashesBase64",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog");

            migrationBuilder.AlterColumn<int>(
                name: "NewStatus",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
