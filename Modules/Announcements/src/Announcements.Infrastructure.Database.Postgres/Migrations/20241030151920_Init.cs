using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Announcements");

            migrationBuilder.CreateTable(
                name: "Announcements",
                schema: "Announcements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Severity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementText",
                schema: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Language = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementText", x => new { x.AnnouncementId, x.Language });
                    table.ForeignKey(
                        name: "FK_AnnouncementText_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalSchema: "Announcements",
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementText",
                schema: "Announcements");

            migrationBuilder.DropTable(
                name: "Announcements",
                schema: "Announcements");
        }
    }
}
