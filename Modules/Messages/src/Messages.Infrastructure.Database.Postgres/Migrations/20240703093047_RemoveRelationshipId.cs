using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    [DependsOn(ModuleType.Messages, "20240701075023_Init")]
    public partial class RemoveRelationshipId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipientInformation_Relationships_RelationshipId",
                schema: "Messages",
                table: "RecipientInformation");

            migrationBuilder.DropIndex(
                name: "IX_RecipientInformation_RelationshipId",
                schema: "Messages",
                table: "RecipientInformation");

            migrationBuilder.DropColumn(
                name: "RelationshipId",
                schema: "Messages",
                table: "RecipientInformation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelationshipId",
                schema: "Messages",
                table: "RecipientInformation",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientInformation_RelationshipId",
                schema: "Messages",
                table: "RecipientInformation",
                column: "RelationshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipientInformation_Relationships_RelationshipId",
                schema: "Messages",
                table: "RecipientInformation",
                column: "RelationshipId",
                principalSchema: "Relationships",
                principalTable: "Relationships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
