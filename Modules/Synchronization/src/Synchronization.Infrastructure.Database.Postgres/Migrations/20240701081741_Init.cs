﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Synchronization");

            migrationBuilder.CreateTable(
                name: "Datawallets",
                schema: "Synchronization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Owner = table.Column<string>(type: "character varying(80)", unicode: false, maxLength: 80, nullable: false),
                    Version = table.Column<int>(type: "integer", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datawallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncRuns",
                schema: "Synchronization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    FinalizedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EventCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatawalletModifications",
                schema: "Synchronization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    DatawalletId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    DatawalletVersion = table.Column<int>(type: "integer", unicode: false, nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false),
                    ObjectIdentifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PayloadCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Collection = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EncryptedPayload = table.Column<byte[]>(type: "bytea", nullable: true),
                    BlobReference = table.Column<string>(type: "character(32)", unicode: false, fixedLength: true, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatawalletModifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatawalletModifications_Datawallets_DatawalletId",
                        column: x => x.DatawalletId,
                        principalSchema: "Synchronization",
                        principalTable: "Datawallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalEvents",
                schema: "Synchronization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false),
                    Owner = table.Column<string>(type: "character varying(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Payload = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SyncErrorCount = table.Column<byte>(type: "smallint", nullable: false),
                    SyncRunId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalEvents_SyncRuns_SyncRunId",
                        column: x => x.SyncRunId,
                        principalSchema: "Synchronization",
                        principalTable: "SyncRuns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SyncErrors",
                schema: "Synchronization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    SyncRunId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    ExternalEventId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    ErrorCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncErrors_ExternalEvents_ExternalEventId",
                        column: x => x.ExternalEventId,
                        principalSchema: "Synchronization",
                        principalTable: "ExternalEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SyncErrors_SyncRuns_SyncRunId",
                        column: x => x.SyncRunId,
                        principalSchema: "Synchronization",
                        principalTable: "SyncRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatawalletModifications_CreatedBy_Index",
                schema: "Synchronization",
                table: "DatawalletModifications",
                columns: new[] { "CreatedBy", "Index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatawalletModifications_DatawalletId",
                schema: "Synchronization",
                table: "DatawalletModifications",
                column: "DatawalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Datawallets_Owner",
                schema: "Synchronization",
                table: "Datawallets",
                column: "Owner",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalEvents_Owner_Index",
                schema: "Synchronization",
                table: "ExternalEvents",
                columns: new[] { "Owner", "Index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalEvents_Owner_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents",
                columns: new[] { "Owner", "SyncRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalEvents_SyncRunId",
                schema: "Synchronization",
                table: "ExternalEvents",
                column: "SyncRunId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncErrors_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors",
                column: "ExternalEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncErrors_SyncRunId_ExternalEventId",
                schema: "Synchronization",
                table: "SyncErrors",
                columns: new[] { "SyncRunId", "ExternalEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncRuns_CreatedBy_FinalizedAt",
                schema: "Synchronization",
                table: "SyncRuns",
                columns: new[] { "CreatedBy", "FinalizedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SyncRuns_CreatedBy_Index",
                schema: "Synchronization",
                table: "SyncRuns",
                columns: new[] { "CreatedBy", "Index" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatawalletModifications",
                schema: "Synchronization");

            migrationBuilder.DropTable(
                name: "SyncErrors",
                schema: "Synchronization");

            migrationBuilder.DropTable(
                name: "Datawallets",
                schema: "Synchronization");

            migrationBuilder.DropTable(
                name: "ExternalEvents",
                schema: "Synchronization");

            migrationBuilder.DropTable(
                name: "SyncRuns",
                schema: "Synchronization");
        }
    }
}
