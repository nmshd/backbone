using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class asd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.DropIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Synchronization",
                table: "SyncErrors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql(
                @"UPDATE ""Synchronization"".""SyncErrors""
                  SET ""CreatedAt"" = sr.""CreatedAt""
                  FROM ""Synchronization"".""SyncRuns"" sr
                  WHERE ""SyncRunId"" = sr.""Id"";");

            migrationBuilder.DropColumn(
                name: "SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Synchronization",
                table: "SyncErrors");

            migrationBuilder.AddColumn<string>(
                name: "SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors",
                columns: new[] { "SyncRunId", "ExternalEventId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SyncErrors_SyncRuns_SyncRunId",
                schema: "Synchronization",
                table: "SyncErrors",
                column: "SyncRunId",
                principalSchema: "Synchronization",
                principalTable: "SyncRuns",
                principalColumn: "Id");
        }
    }
}
