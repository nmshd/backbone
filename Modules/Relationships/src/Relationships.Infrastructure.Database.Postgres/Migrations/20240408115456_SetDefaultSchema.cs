using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Relationships");

            migrationBuilder.RenameTable(
                name: "RelationshipTemplates",
                newName: "RelationshipTemplates",
                newSchema: "Relationships");

            migrationBuilder.RenameTable(
                name: "RelationshipTemplateAllocations",
                newName: "RelationshipTemplateAllocations",
                newSchema: "Relationships");

            migrationBuilder.RenameTable(
                name: "Relationships",
                newName: "Relationships",
                newSchema: "Relationships");

            migrationBuilder.RenameTable(
                name: "RelationshipChanges",
                newName: "RelationshipChanges",
                newSchema: "Relationships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "RelationshipTemplates",
                schema: "Relationships",
                newName: "RelationshipTemplates");

            migrationBuilder.RenameTable(
                name: "RelationshipTemplateAllocations",
                schema: "Relationships",
                newName: "RelationshipTemplateAllocations");

            migrationBuilder.RenameTable(
                name: "Relationships",
                schema: "Relationships",
                newName: "Relationships");

            migrationBuilder.RenameTable(
                name: "RelationshipChanges",
                schema: "Relationships",
                newName: "RelationshipChanges");
        }
    }
}
