using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnouncementRecipients",
                schema: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementRecipients", x => new { x.AnnouncementId, x.Address });
                    table.ForeignKey(
                        name: "FK_AnnouncementRecipients_Announcements_AnnouncementId",
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
                name: "AnnouncementRecipients",
                schema: "Announcements");
        }
    }
}
