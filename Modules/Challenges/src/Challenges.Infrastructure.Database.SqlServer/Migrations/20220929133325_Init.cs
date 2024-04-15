using Microsoft.EntityFrameworkCore.Migrations;


namespace Backbone.Modules.Challenges.Infrastructure.Database.SqlServer.Migrations;

public partial class Init : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Challenges",
            schema: "Challenges",
            columns: table => new
            {
                Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Challenges", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            schema: "Challenges",
            name: "Challenges");
    }
}
