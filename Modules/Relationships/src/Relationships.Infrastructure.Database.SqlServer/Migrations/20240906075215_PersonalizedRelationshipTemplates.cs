using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class PersonalizedRelationshipTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForIdentity",
                schema: "Relationships",
                table: "RelationshipTemplates",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForIdentity",
                schema: "Relationships",
                table: "RelationshipTemplates");
        }
    }
}
