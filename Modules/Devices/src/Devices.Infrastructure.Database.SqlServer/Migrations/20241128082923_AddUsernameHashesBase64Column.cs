using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameHashesBase64Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NewStatus",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UsernameHashesBase64",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                type: "nvarchar(max)",
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
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
