using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureRelationshipAuditLogDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationshipAuditLog_Relationships_RelationshipId",
                schema: "Relationships",
                table: "RelationshipAuditLog");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationshipAuditLog_Relationships_RelationshipId",
                schema: "Relationships",
                table: "RelationshipAuditLog",
                column: "RelationshipId",
                principalSchema: "Relationships",
                principalTable: "Relationships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationshipAuditLog_Relationships_RelationshipId",
                schema: "Relationships",
                table: "RelationshipAuditLog");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationshipAuditLog_Relationships_RelationshipId",
                schema: "Relationships",
                table: "RelationshipAuditLog",
                column: "RelationshipId",
                principalSchema: "Relationships",
                principalTable: "Relationships",
                principalColumn: "Id");
        }
    }
}
