using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsRevamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 ALTER TABLE [Relationships].[RelationshipChanges] DROP CONSTRAINT [FK_RelationshipChanges_Relationships_RelationshipId]
                 GO
                 
                 DROP VIEW [AdminUi].[RelationshipOverviews]
                 GO
                 
                 DROP TABLE [Relationships].[RelationshipChanges]
                 GO
            """);

            migrationBuilder.AddColumn<byte[]>(
                name: "AcceptanceContent",
                table: "Relationships",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CreationContent",
                table: "Relationships",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RelationshipAuditLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                    CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelationshipId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
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
                name: "AcceptanceContent",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "CreationContent",
                table: "Relationships");

            migrationBuilder.CreateTable(
                name: "RelationshipChanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RelationshipId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Req_Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Req_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Req_CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                    Req_CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Res_Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Res_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Res_CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                    Res_CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationshipChanges_Relationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "Relationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipChanges_RelationshipId",
                table: "RelationshipChanges",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipChanges_Res_CreatedAt_Res_CreatedBy_Res_CreatedByDevice",
                table: "RelationshipChanges",
                columns: new[] { "Res_CreatedAt", "Res_CreatedBy", "Res_CreatedByDevice" })
                .Annotation("SqlServer:Include", new[] { "Res_Content" });
        }
    }
}
