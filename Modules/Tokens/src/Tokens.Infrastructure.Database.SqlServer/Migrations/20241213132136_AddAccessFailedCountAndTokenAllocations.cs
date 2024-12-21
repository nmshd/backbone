using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Tokens.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessFailedCountAndTokenAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                schema: "Tokens",
                table: "Tokens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TokenAllocations",
                schema: "Tokens",
                columns: table => new
                {
                    TokenId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    AllocatedBy = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    AllocatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    AllocatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenAllocations", x => new { x.TokenId, x.AllocatedBy });
                    table.ForeignKey(
                        name: "FK_TokenAllocations_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "Tokens",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenAllocations",
                schema: "Tokens");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                schema: "Tokens",
                table: "Tokens");
        }
    }
}
