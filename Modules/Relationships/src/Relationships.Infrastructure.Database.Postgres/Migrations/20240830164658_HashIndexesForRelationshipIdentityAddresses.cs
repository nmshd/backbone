using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class HashIndexesForRelationshipIdentityAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Relationships_From",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_To",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_From",
                schema: "Relationships",
                table: "Relationships",
                column: "From")
                .Annotation("Npgsql:IndexMethod", "hash");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_To",
                schema: "Relationships",
                table: "Relationships",
                column: "To")
                .Annotation("Npgsql:IndexMethod", "hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Relationships_From",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_To",
                schema: "Relationships",
                table: "Relationships");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_From",
                schema: "Relationships",
                table: "Relationships",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_To",
                schema: "Relationships",
                table: "Relationships",
                column: "To");
        }
    }
}
