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
            migrationBuilder.DropTable(
                schema: "Relationships",
                name: "RelationshipChanges");

            migrationBuilder.AddColumn<byte[]>(
                name: "AcceptanceContent",
                schema: "Relationships",
                table: "Relationships",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CreationContent",
                schema: "Relationships",
                table: "Relationships",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateTable(
                schema: "Relationships",
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
                        principalSchema: "Relationships",
                        principalTable: "Relationships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipAuditLog_RelationshipId",
                schema: "Relationships",
                table: "RelationshipAuditLog",
                column: "RelationshipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                schema: "Relationships",
                name: "RelationshipAuditLog");

            migrationBuilder.DropColumn(
                name: "AcceptanceContent",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "CreationContent",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.CreateTable(
                schema: "Relationships",
                name: "RelationshipChanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RelationshipId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Req_Content = table.Column<byte[]>(type: "bytea", nullable: true),
                    Req_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Req_CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                    Req_CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Res_Content = table.Column<byte[]>(type: "bytea", nullable: true),
                    Res_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Res_CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                    Res_CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationshipChanges_Relationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalSchema: "Relationships",
                        principalTable: "Relationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipChanges_RelationshipId",
                schema: "Relationships",
                table: "RelationshipChanges",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipChanges_Res_CreatedAt_Res_CreatedBy_Res_Created~",
                schema: "Relationships",
                table: "RelationshipChanges",
                columns: new[] { "Res_CreatedAt", "Res_CreatedBy", "Res_CreatedByDevice" })
                .Annotation("Npgsql:IndexInclude", new[] { "Res_Content" });
        }
    }
}
