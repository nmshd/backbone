using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RelationshipTemplates_CreatedBy",
                table: "RelationshipTemplates");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipTemplates_DeletedAt",
                table: "RelationshipTemplates");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipTemplates_ExpiresAt",
                table: "RelationshipTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_CreatedAt",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_Status",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_CreatedAt",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Req_CreatedAt",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Req_CreatedBy",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Req_CreatedByDevice",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Res_CreatedAt",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Res_CreatedBy",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Res_CreatedByDevice",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Status",
                table: "RelationshipChanges");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Type",
                table: "RelationshipChanges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_CreatedAt",
                table: "Relationships",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_Status",
                table: "Relationships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipChanges_CreatedAt",
                table: "RelationshipChanges",
                column: "CreatedAt");

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
        }
    }
}
