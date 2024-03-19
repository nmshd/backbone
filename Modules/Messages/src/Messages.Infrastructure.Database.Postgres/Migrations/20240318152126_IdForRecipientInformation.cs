using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdForRecipientInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RecipientInformation",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientInformation_Address_MessageId",
                table: "RecipientInformation",
                columns: new[] { "Address", "MessageId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation");

            migrationBuilder.DropIndex(
                name: "IX_RecipientInformation_Address_MessageId",
                table: "RecipientInformation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RecipientInformation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation",
                columns: new[] { "Address", "MessageId" });
        }
    }
}
