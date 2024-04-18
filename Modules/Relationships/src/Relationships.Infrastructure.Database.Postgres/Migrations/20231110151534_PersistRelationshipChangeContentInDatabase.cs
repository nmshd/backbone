using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class PersistRelationshipChangeContentInDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                schema: "Relationships",
                table: "RelationshipTemplates",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Req_Content",
                schema: "Relationships",
                table: "RelationshipChanges",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Res_Content",
                schema: "Relationships",
                table: "RelationshipChanges",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                schema: "Relationships",
                table: "RelationshipTemplates");

            migrationBuilder.DropColumn(
                name: "Req_Content",
                schema: "Relationships",
                table: "RelationshipChanges");

            migrationBuilder.DropColumn(
                name: "Res_Content",
                schema: "Relationships",
                table: "RelationshipChanges");
        }
    }
}
