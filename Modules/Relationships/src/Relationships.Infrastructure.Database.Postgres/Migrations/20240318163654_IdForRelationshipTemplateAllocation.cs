using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdForRelationshipTemplateAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RelationshipTemplateAllocations",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                schema: "Relationships",
                table: "RelationshipChanges",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RelationshipTemplateAllocations",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipTemplateAllocations_RelationshipTemplateId_Allo~",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations",
                columns: new[] { "RelationshipTemplateId", "AllocatedBy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RelationshipTemplateAllocations",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations");

            migrationBuilder.DropIndex(
                name: "IX_RelationshipTemplateAllocations_RelationshipTemplateId_Allo~",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                schema: "Relationships",
                table: "RelationshipChanges",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(34)",
                oldMaxLength: 34);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RelationshipTemplateAllocations",
                schema: "Relationships",
                table: "RelationshipTemplateAllocations",
                columns: new[] { "RelationshipTemplateId", "AllocatedBy" });
        }
    }
}
