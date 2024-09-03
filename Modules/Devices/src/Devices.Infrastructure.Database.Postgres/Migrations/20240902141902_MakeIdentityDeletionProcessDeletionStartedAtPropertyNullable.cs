using System;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    [DependsOn(ModuleType.Devices, "20240830164312_HashIndexesForIds")]
    public partial class MakeIdentityDeletionProcessDeletionStartedAtPropertyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletionStartedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.Sql("""
                                    UPDATE "Devices"."IdentityDeletionProcesses"
                                    SET "DeletionStartedAt" = NULL
                                    WHERE "DeletionStartedAt" = '-infinity'::timestamp with time zone;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletionStartedAt",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
