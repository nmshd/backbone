using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Relationships");

            migrationBuilder.CreateTable(
                name: "RelationshipTemplates",
                schema: "Relationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    MaxNumberOfAllocations = table.Column<int>(type: "int", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relationships",
                schema: "Relationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    RelationshipTemplateId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    From = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    To = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreationResponseContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FromHasDecomposed = table.Column<bool>(type: "bit", nullable: false),
                    ToHasDecomposed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                        column: x => x.RelationshipTemplateId,
                        principalSchema: "Relationships",
                        principalTable: "RelationshipTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelationshipTemplateAllocations",
                schema: "Relationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelationshipTemplateId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    AllocatedBy = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    AllocatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllocatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipTemplateAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_RelationshipTemplateId",
                        column: x => x.RelationshipTemplateId,
                        principalSchema: "Relationships",
                        principalTable: "RelationshipTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelationshipAuditLog",
                schema: "Relationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
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
                        principalSchema: "Relationships",
                        principalTable: "Relationships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipAuditLog_RelationshipId",
                schema: "Relationships",
                table: "RelationshipAuditLog",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_From",
                schema: "Relationships",
                table: "Relationships",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_RelationshipTemplateId",
                schema: "Relationships",
                table: "Relationships",
                column: "RelationshipTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_To",
                schema: "Relationships",
                table: "Relationships",
                column: "To");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipTemplateAllocations_RelationshipTemplateId_AllocatedBy",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations",
                columns: new[] { "RelationshipTemplateId", "AllocatedBy" });

            migrationBuilder.AddCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities();
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelationshipAuditLog",
                schema: "Relationships");

            migrationBuilder.DropTable(
                name: "RelationshipTemplateAllocations",
                schema: "Relationships");

            migrationBuilder.DropTable(
                name: "Relationships",
                schema: "Relationships");

            migrationBuilder.DropTable(
                name: "RelationshipTemplates",
                schema: "Relationships");
        }
    }
}
