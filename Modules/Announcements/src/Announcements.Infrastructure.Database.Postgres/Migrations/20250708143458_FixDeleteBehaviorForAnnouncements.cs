using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FixDeleteBehaviorForAnnouncements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementActions_Announcements_AnnouncementId",
                schema: "Announcements",
                table: "AnnouncementActions");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementActions_Announcements_AnnouncementId",
                schema: "Announcements",
                table: "AnnouncementActions",
                column: "AnnouncementId",
                principalSchema: "Announcements",
                principalTable: "Announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementActions_Announcements_AnnouncementId",
                schema: "Announcements",
                table: "AnnouncementActions");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementActions_Announcements_AnnouncementId",
                schema: "Announcements",
                table: "AnnouncementActions",
                column: "AnnouncementId",
                principalSchema: "Announcements",
                principalTable: "Announcements",
                principalColumn: "Id");
        }
    }
}
