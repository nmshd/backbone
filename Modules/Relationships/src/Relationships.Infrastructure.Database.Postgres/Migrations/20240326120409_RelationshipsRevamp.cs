using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsRevamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This is commented out because the RelationshipsOverview view of the Admin UI depends on it.
            // But since we will combine all migrations into one soon anyway, we can ignore this for now.
            // migrationBuilder.DropTable(
            //     name: "RelationshipChanges");

            migrationBuilder.AddColumn<byte[]>(
                name: "CreationContent",
                table: "Relationships",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RelationshipAuditLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    OldStatus = table.Column<int>(type: "integer", nullable: true),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                    CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RelationshipId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationshipAuditLog_Relationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "Relationships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipAuditLog_RelationshipId",
                table: "RelationshipAuditLog",
                column: "RelationshipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelationshipAuditLog");

            migrationBuilder.DropColumn(
                name: "CreationContent",
                table: "Relationships");

            // This is commented out because the RelationshipsOverview view of the Admin UI depends on it.
            // But since we will combine all migrations into one soon anyway, we can ignore this for now.
            // migrationBuilder.CreateTable(
            //     name: "RelationshipChanges",
            //     columns: table => new
            //     {
            //         Id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            //         RelationshipId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
            //         CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //         Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
            //         Status = table.Column<int>(type: "integer", nullable: false),
            //         Type = table.Column<int>(type: "integer", nullable: false),
            //         Req_Content = table.Column<byte[]>(type: "bytea", nullable: true),
            //         Req_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //         Req_CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
            //         Req_CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
            //         Res_Content = table.Column<byte[]>(type: "bytea", nullable: true),
            //         Res_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //         Res_CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
            //         Res_CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_RelationshipChanges", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_RelationshipChanges_Relationships_RelationshipId",
            //             column: x => x.RelationshipId,
            //             principalTable: "Relationships",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_CreatedAt",
            //     table: "RelationshipChanges",
            //     column: "CreatedAt");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_RelationshipId",
            //     table: "RelationshipChanges",
            //     column: "RelationshipId");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Req_CreatedAt",
            //     table: "RelationshipChanges",
            //     column: "Req_CreatedAt");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Req_CreatedBy",
            //     table: "RelationshipChanges",
            //     column: "Req_CreatedBy");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Req_CreatedByDevice",
            //     table: "RelationshipChanges",
            //     column: "Req_CreatedByDevice");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Res_CreatedAt",
            //     table: "RelationshipChanges",
            //     column: "Res_CreatedAt");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Res_CreatedBy",
            //     table: "RelationshipChanges",
            //     column: "Res_CreatedBy");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Res_CreatedByDevice",
            //     table: "RelationshipChanges",
            //     column: "Res_CreatedByDevice");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Status",
            //     table: "RelationshipChanges",
            //     column: "Status");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_RelationshipChanges_Type",
            //     table: "RelationshipChanges",
            //     column: "Type");
        }
    }
}
