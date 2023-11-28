using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipTemplateIdNotNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                table: "Relationships");

            migrationBuilder.AlterColumn<string>(
                name: "RelationshipTemplateId",
                table: "Relationships",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                table: "Relationships",
                column: "RelationshipTemplateId",
                principalTable: "RelationshipTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                table: "Relationships");

            migrationBuilder.AlterColumn<string>(
                name: "RelationshipTemplateId",
                table: "Relationships",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_RelationshipTemplates_RelationshipTemplateId",
                table: "Relationships",
                column: "RelationshipTemplateId",
                principalTable: "RelationshipTemplates",
                principalColumn: "Id");
        }
    }
}
