using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Tokens.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Tokens");

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "Tokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "Tokens");
        }
    }
}
