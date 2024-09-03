using System;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    [DependsOn(ModuleType.Devices, "20240708114346_AddAdditionalDataToIdentityDeletionProcessAuditLogEntry")]
    public partial class MakeIdentityDeletionProcessDeletionStartedAtPropertyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletionStartedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.Sql("""
                                     UPDATE [Devices].[IdentityDeletionProcesses]
                                     SET [DeletionStartedAt] = NULL
                                     WHERE [DeletionStartedAt] = '0001-01-01 00:00:00.0000000';
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletionStartedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
