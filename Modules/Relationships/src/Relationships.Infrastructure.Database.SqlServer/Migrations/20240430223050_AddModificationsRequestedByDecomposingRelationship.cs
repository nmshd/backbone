using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddModificationsRequestedByDecomposingRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FromHasDecomposed",
                schema: "Relationships",
                table: "Relationships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ToHasDecomposed",
                schema: "Relationships",
                table: "Relationships",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromHasDecomposed",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "ToHasDecomposed",
                schema: "Relationships",
                table: "Relationships");
        }
    }
}
