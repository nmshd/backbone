using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Init_Quotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tiers",
                schema: "Quotas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Identities",
                schema: "Quotas",
                columns: table => new
                {
                    Address = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                    TierId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Address);
                    table.ForeignKey(
                        name: "FK_Identities_Tiers_TierId",
                        column: x => x.TierId,
                        principalSchema: "Quotas",
                        principalTable: "Tiers",
                        principalColumn: "Id");
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Identities_TierId",
                schema: "Quotas",
                table: "Identities",
                column: "TierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                schema: "Quotas",
                name: "Identities");

            migrationBuilder.RenameTable(
                schema: "Quotas",
                name: "Tiers");
        }
    }
}
