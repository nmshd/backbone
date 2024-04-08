using Backbone.Modules.Relationships.Infrastructure.Database.SqlServer;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relationships.Infrastructure.Database.SqlServer.Migrations;

public partial class Init : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RelationshipTemplates",
            schema: "Relationships",
            columns: table => new
            {
                Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                MaxNumberOfAllocations = table.Column<int>(type: "int", nullable: true),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RelationshipTemplates", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "Relationships",
            schema: "Relationships",
            columns: table => new
            {
                Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                RelationshipTemplateId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                From = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                To = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Relationships", x => x.Id);
                table.ForeignKey(
                    name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                    column: x => x.RelationshipTemplateId,
                    principalTable: "RelationshipTemplates",
                    principalColumn: "Id");
            }
        );

        migrationBuilder.CreateTable(
            name: "RelationshipTemplateAllocations",
            schema: "Relationships",
            columns: table => new
            {
                RelationshipTemplateId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                AllocatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                AllocatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                AllocatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RelationshipTemplateAllocations", x => new { x.RelationshipTemplateId, x.AllocatedBy });
                table.ForeignKey(
                    name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_RelationshipTemplateId",
                    column: x => x.RelationshipTemplateId,
                    principalSchema: "Relationships",
                    principalTable: "RelationshipTemplates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "RelationshipChanges",
            schema: "Relationships",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                RelationshipId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Req_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Req_CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                Req_CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                Res_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Res_CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                Res_CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_CreatedAt",
            table: "RelationshipChanges",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_RelationshipId",
            table: "RelationshipChanges",
            column: "RelationshipId");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Req_CreatedAt",
            table: "RelationshipChanges",
            column: "Req_CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Req_CreatedBy",
            table: "RelationshipChanges",
            column: "Req_CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Req_CreatedByDevice",
            table: "RelationshipChanges",
            column: "Req_CreatedByDevice");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Res_CreatedAt",
            table: "RelationshipChanges",
            column: "Res_CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Res_CreatedBy",
            table: "RelationshipChanges",
            column: "Res_CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Res_CreatedByDevice",
            table: "RelationshipChanges",
            column: "Res_CreatedByDevice");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Status",
            table: "RelationshipChanges",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Type",
            table: "RelationshipChanges",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_CreatedAt",
            table: "Relationships",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_From",
            table: "Relationships",
            column: "From");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_RelationshipTemplateId",
            table: "Relationships",
            column: "RelationshipTemplateId");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_Status",
            table: "Relationships",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_To",
            table: "Relationships",
            column: "To");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipTemplates_CreatedBy",
            table: "RelationshipTemplates",
            column: "CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipTemplates_DeletedAt",
            table: "RelationshipTemplates",
            column: "DeletedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipTemplates_ExpiresAt",
            table: "RelationshipTemplates",
            column: "ExpiresAt");

        migrationBuilder.AddCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities();

        migrationBuilder.DropTable(
            name: "RelationshipChanges");

        migrationBuilder.DropTable(
            name: "RelationshipTemplateAllocations");

        migrationBuilder.DropTable(
            name: "Relationships");

        migrationBuilder.DropTable(
            name: "RelationshipTemplates");
    }
}
