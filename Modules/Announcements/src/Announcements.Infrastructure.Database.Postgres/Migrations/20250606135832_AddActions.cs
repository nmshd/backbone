using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnouncementActions",
                schema: "Announcements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Link = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    AnnouncementId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementActions_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalSchema: "Announcements",
                        principalTable: "Announcements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementActions_AnnouncementId",
                schema: "Announcements",
                table: "AnnouncementActions",
                column: "AnnouncementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementActions",
                schema: "Announcements");
        }
    }
}
