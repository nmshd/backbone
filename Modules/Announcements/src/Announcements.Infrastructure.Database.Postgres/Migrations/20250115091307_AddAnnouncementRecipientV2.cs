using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementRecipientV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnouncementRecipients",
                schema: "Announcements",
                table: "AnnouncementRecipients");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                schema: "Announcements",
                table: "AnnouncementRecipients");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Announcements",
                table: "AnnouncementRecipients",
                type: "character varying(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnouncementRecipients",
                schema: "Announcements",
                table: "AnnouncementRecipients",
                columns: new[] { "AnnouncementId", "Address" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnouncementRecipients",
                schema: "Announcements",
                table: "AnnouncementRecipients");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Announcements",
                table: "AnnouncementRecipients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldUnicode: false,
                oldMaxLength: 80);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                schema: "Announcements",
                table: "AnnouncementRecipients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnouncementRecipients",
                schema: "Announcements",
                table: "AnnouncementRecipients",
                columns: new[] { "AnnouncementId", "DeviceId", "Address" });
        }
    }
}
