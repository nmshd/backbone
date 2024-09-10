﻿using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    [DependsOn(ModuleType.Devices, "20240902141900_MakeIdentityDeletionProcessDeletionStartedAtPropertyNullable")]
    public partial class AddUniqueIndexOnActiveDeletionProcesses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldUnicode: false,
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_only_one_active_deletion_process",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                unique: true,
                filter: "\"Status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_only_one_active_deletion_process",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldUnicode: false,
                oldMaxLength: 80);
        }
    }
}
