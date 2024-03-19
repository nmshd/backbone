﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class DoNotDropRelationshipsTemplateAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_RelationshipTemplateId",
                table: "RelationshipTemplateAllocations");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_RelationshipTemplateId",
                table: "RelationshipTemplateAllocations",
                column: "RelationshipTemplateId",
                principalTable: "RelationshipTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_RelationshipTemplateId",
                table: "RelationshipTemplateAllocations");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationshipTemplateAllocations_RelationshipTemplates_RelationshipTemplateId",
                table: "RelationshipTemplateAllocations",
                column: "RelationshipTemplateId",
                principalTable: "RelationshipTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
