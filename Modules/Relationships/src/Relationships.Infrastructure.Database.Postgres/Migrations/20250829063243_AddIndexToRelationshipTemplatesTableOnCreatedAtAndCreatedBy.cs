using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToRelationshipTemplatesTableOnCreatedAtAndCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RelationshipTemplates_CreatedAt",
                schema: "Relationships",
                table: "RelationshipTemplates",
                column: "CreatedAt")
                .Annotation("Npgsql:IndexInclude", new[] { "CreatedBy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RelationshipTemplates_CreatedAt",
                schema: "Relationships",
                table: "RelationshipTemplates");
        }
    }
}
