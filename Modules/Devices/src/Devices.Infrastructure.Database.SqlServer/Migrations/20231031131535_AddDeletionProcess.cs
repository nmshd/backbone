﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletionProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcesses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentityAddress = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDeletionProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                        column: x => x.IdentityAddress,
                        principalTable: "Identities",
                        principalColumn: "Address");
                });

            migrationBuilder.CreateTable(
                name: "IdentityDeletionProcessAuditLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityAddressHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DeviceIdHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    OldStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    IdentityDeletionProcessId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDeletionProcessAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_IdentityDeletionProcessId",
                        column: x => x.IdentityDeletionProcessId,
                        principalTable: "IdentityDeletionProcesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDeletionProcessAuditLog_IdentityDeletionProcessId",
                table: "IdentityDeletionProcessAuditLog",
                column: "IdentityDeletionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDeletionProcesses_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityDeletionProcessAuditLog");

            migrationBuilder.DropTable(
                name: "IdentityDeletionProcesses");
        }
    }
}
