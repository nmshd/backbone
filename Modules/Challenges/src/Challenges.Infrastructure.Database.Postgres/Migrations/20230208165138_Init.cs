using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challenges.Infrastructure.Database.Postgres.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Challenges",
            columns: table => new
            {
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Challenges", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Challenges");
    }
}
