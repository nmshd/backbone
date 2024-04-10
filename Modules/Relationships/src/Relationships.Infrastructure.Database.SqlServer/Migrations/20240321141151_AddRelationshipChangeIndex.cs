﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipChangeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RelationshipChanges_Res_CreatedAt_Res_CreatedBy_Res_CreatedByDevice",
                table: "RelationshipChanges",
                columns: new[] { "Res_CreatedAt", "Res_CreatedBy", "Res_CreatedByDevice" })
                .Annotation("SqlServer:Include", new[] { "Res_Content" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RelationshipChanges_Res_CreatedAt_Res_CreatedBy_Res_CreatedByDevice",
                table: "RelationshipChanges");
        }
    }
}
