using Backbone.Modules.Relationships.Infrastructure.Database.Postgres;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Relationships.Infrastructure.Database.Postgres.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RelationshipTemplates",
            schema: "Relationships",
            columns: table => new
            {
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                MaxNumberOfAllocations = table.Column<int>(type: "integer", nullable: true),
                ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                RelationshipTemplateId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                From = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                To = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Relationships", x => x.Id);
                table.ForeignKey(
                    name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                    column: x => x.RelationshipTemplateId,
                    principalSchema: "Relationships",
                    principalTable: "RelationshipTemplates",
                    principalColumn: "Id");
            }
        );

        migrationBuilder.CreateTable(
            name: "RelationshipTemplateAllocations",
            schema: "Relationships",
            columns: table => new
            {
                RelationshipTemplateId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                AllocatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                AllocatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                AllocatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RelationshipTemplateAllocations", x => new { x.RelationshipTemplateId, x.AllocatedBy });
                table.ForeignKey(
                    name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_Relat~",
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
                Id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                RelationshipId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ReqCreatedAt = table.Column<DateTime>(name: "Req_CreatedAt", type: "timestamp with time zone", nullable: false),
                ReqCreatedBy = table.Column<string>(name: "Req_CreatedBy", type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                ReqCreatedByDevice = table.Column<string>(name: "Req_CreatedByDevice", type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                ResCreatedAt = table.Column<DateTime>(name: "Res_CreatedAt", type: "timestamp with time zone", nullable: true),
                ResCreatedBy = table.Column<string>(name: "Res_CreatedBy", type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                ResCreatedByDevice = table.Column<string>(name: "Res_CreatedByDevice", type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                Discriminator = table.Column<string>(type: "text", nullable: false)
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
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_RelationshipId",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "RelationshipId");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Req_CreatedAt",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Req_CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Req_CreatedBy",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Req_CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Req_CreatedByDevice",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Req_CreatedByDevice");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Res_CreatedAt",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Res_CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Res_CreatedBy",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Res_CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Res_CreatedByDevice",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Res_CreatedByDevice");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Status",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipChanges_Type",
            schema: "Relationships",
            table: "RelationshipChanges",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_CreatedAt",
            schema: "Relationships",
            table: "Relationships",
            column: "CreatedAt");

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
            name: "IX_Relationships_Status",
            schema: "Relationships",
            table: "Relationships",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Relationships_To",
            schema: "Relationships",
            table: "Relationships",
            column: "To");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipTemplates_CreatedBy",
            schema: "Relationships",
            table: "RelationshipTemplates",
            column: "CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipTemplates_DeletedAt",
            schema: "Relationships",
            table: "RelationshipTemplates",
            column: "DeletedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RelationshipTemplates_ExpiresAt",
            schema: "Relationships",
            table: "RelationshipTemplates",
            column: "ExpiresAt");

        migrationBuilder.AddCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities();
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteCheckConstraintForAtMostOneRelationshipBetweenTwoIdentities();

        migrationBuilder.DropTable(
            schema: "Relationships",
            name: "RelationshipChanges");

        migrationBuilder.DropTable(
            schema: "Relationships",
            name: "RelationshipTemplateAllocations");

        migrationBuilder.DropTable(
            schema: "Relationships",
            name: "Relationships");

        migrationBuilder.DropTable(
            schema: "Relationships",
            name: "RelationshipTemplates");
    }
}
