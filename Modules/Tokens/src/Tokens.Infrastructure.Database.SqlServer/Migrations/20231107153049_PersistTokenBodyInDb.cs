using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Tokens.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class PersistTokenBodyInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Tokens",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Tokens");
        }
    }
}
